using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Explorip.HookFileOperations.Models;

namespace Explorip.HookFileOperations
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    public class ServerInterface : MarshalByRefObject
    {
        private readonly List<OneFileOperation> _listOperations = new();

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

        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
            // Just to test if server still exist
        }

        public void CopyItem(string src, string dest, string destName)
        {
            Console.WriteLine("Add CopyItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Copy) { Source = src, Destination = dest, NewName = destName });
        }

        public void MoveItem(string src, string dest, string destName)
        {
            Console.WriteLine("Add MoveItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Move) { Source = src, Destination = dest, NewName = destName });
        }

        public void DeleteItem(string src)
        {
            Console.WriteLine("Add DeleteItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Delete) { Source = src });
        }

        public void RenameItem(string src, string dest)
        {
            Console.WriteLine("Add RenameItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Rename) { Source = src, NewName = dest });
        }

        public void PerformOperations()
        {
            Console.WriteLine("PerformOperation");
            if (_listOperations.Count > 0)
            {
                Console.WriteLine("Create process");
                Process newProcess = Process.Start("HookFileOperationsManager.exe", "NI");
                Thread.Sleep(100);
                IpcNewInstance ni = null;
                Console.WriteLine("Wait for Ipc channel");
                while (ni == null)
                {
                    Thread.Sleep(10);
                    try
                    {
                        ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://{"HookFileOperation_" + newProcess.Id.ToString()}/HookManagerRemoteServer");
                    }
                    catch (Exception) { /* Ignore errors */ }
                }
                Console.WriteLine("Wait for process ready");
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
                Console.WriteLine("Send Operation" + Environment.NewLine);
                ni.StartNewFileOperation(_listOperations);
                _listOperations.Clear();
            }
        }

        public void NewItem(string destFolder, FileAttributes dwFileAttributes, string filename)
        {
            Console.WriteLine("Add NewItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Create) { Destination = destFolder, Attributes = dwFileAttributes, NewName = filename });
        }
    }
}
