using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Explorip.HookFileOperations.Models;

using EFileOperation = Explorip.HookFileOperations.Models.EFileOperation;

namespace Explorip.HookFileOperations.Ipc;

/// <summary>
/// Provides an interface for communicating from the client (target) to the server (injector)
/// </summary>
public class ServerInterface : MarshalByRefObject
{
    private readonly List<OneFileOperation> _listOperations = [];

#pragma warning disable IDE0079
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public void IsInstalled(int clientPID)
    {
        Console.WriteLine("Explorip has injected HookFileOperations into process {0}.", clientPID);
    }

    /// <summary>
    /// Output the message to the console.
    /// </summary>
    /// <param name="messages">List of messages to display in the console</param>
    public void ReportMessages(string[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Console.WriteLine(messages[i]);
        }
    }

    public void ReportMessage(string message)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    /// Report exception
    /// </summary>
    /// <param name="e"></param>
    public void ReportException(Exception e)
    {
        Console.WriteLine($"The target process has reported an error:{Environment.NewLine}" + e.ToString());
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
#pragma warning restore IDE0079

    /// <summary>
    /// Called to confirm that the IPC channel is still open / host application has not closed
    /// </summary>
    public void Ping()
    {
        // Just to test if server still exist
    }

    public void CopyItem(string src, string dest, string destName)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Copy) { Source = src, Destination = dest, NewName = destName });
    }

    public void MoveItem(string src, string dest, string destName)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Move) { Source = src, Destination = dest, NewName = destName });
    }

    [DllImport("USER32.dll")]
    private static extern short GetKeyState(int nVirtKey);
    public void DeleteItem(string src)
    {
        bool forceDelete = true;
        short ret = GetKeyState(0x10);
        if (ret == 0 || ret == 1)
            forceDelete = false;
        _listOperations.Add(new OneFileOperation(EFileOperation.Delete) { Source = src, ForceDeleteNoRecycled = forceDelete });
    }

    public void RenameItem(string src, string dest)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Rename) { Source = src, NewName = dest });
    }

    public void PerformOperations()
    {
        if (_listOperations.Count > 0)
        {
            IpcNewInstance ni;
            try
            {
                ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://ExploripCopy/HookManagerRemoteServer");
                if (ni?.ExploripCopyLaunched() != true)
                    ni = null;
            }
            catch (Exception)
            {
                ni = null;
            }
            if (ni == null)
            {
                Process newProcess = Process.Start("HookFileOperationsManager.exe", "NI");
                Thread.Sleep(100);
                while (ni == null)
                {
                    Thread.Sleep(10);
                    try
                    {
                        ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://{"HookFileOperation_" + newProcess.Id.ToString()}/HookManagerRemoteServer");
                    }
                    catch (Exception) { /* Ignore errors */ }
                }
            }
            while (true)
            {
                try
                {
                    while (!ni.IsReady())
                    {
                        Thread.Sleep(10);
                    }
                    if (ni.IsReady())
                        break;
                }
                catch (Exception) { /* Ignore errors */ }
            }
            ni.StartNewFileOperation(_listOperations);
            _listOperations.Clear();
        }
    }

    public void NewItem(string destFolder, FileAttributes dwFileAttributes, string filename)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Create) { Destination = destFolder, Attributes = dwFileAttributes, NewName = filename });
    }

    public void SetOperationFlags(FilesOperations.EFileOperation operationFlag)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.ChangeOperationFlags) { OperationsFlags = operationFlag });
    }

    public void ApplyPropertiesToItem(string src)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.ApplyProperties) { Source = src });
    }

    public void Advice()
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Advice));
    }

    public void Unadvice()
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.Unadvice));
    }

    public void SetProperties(object properties)
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.SetProperties) { Properties = properties });
    }

    public void SetProgressDialog()
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.ProgressDialog));
    }

    public void SetProgressMessage()
    {
        _listOperations.Add(new OneFileOperation(EFileOperation.ProgressMessage));
    }
}
