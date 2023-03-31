using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetDesktopWindow();

        public void PerformOperations()
        {
            Console.WriteLine("PerformOperation");
            Task.Run(() =>
            {
                Console.WriteLine("Start Task");
                if (_listOperations.Count > 0)
                {
                    Console.WriteLine("Current AppDomain=" + AppDomain.CurrentDomain.FriendlyName);
                    Console.WriteLine("Create new FileOperation");
                    FileOperation currentFileOperation = new(GetDesktopWindow());
                    Console.WriteLine("FileOperation created");
                    foreach (OneFileOperation ope in _listOperations)
                        ope.WriteOperation(currentFileOperation);
                    _listOperations.Clear();
                    Console.WriteLine("Send PerformOperation" + Environment.NewLine);
                    currentFileOperation.PerformOperations();
                    currentFileOperation.Dispose();
                }
            });
        }

        public uint NewItem(string destFolder, FileAttributes dwFileAttributes, string filename)
        {
            Console.WriteLine("Add NewItem");
            _listOperations.Add(new OneFileOperation(EFileOperation.Create) { Destination = destFolder, Attributes = dwFileAttributes, NewName = filename });
            return 0;
        }
    }
}
