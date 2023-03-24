using System;
using System.IO;
using System.Threading.Tasks;

using Explorip.HookFileOperations.FilesOperations.Interfaces;
using Explorip.HookFileOperations.Helpers;

namespace Explorip.HookFileOperations
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    public class ServerInterface : MarshalByRefObject
    {
        private bool _performDone = true;
        private IFileOperation _currentFileOperation;

        public void IsInstalled(int clientPID)
        {
            Console.WriteLine("Explorip has injected HookFileOperations into process {0}.", clientPID);
        }

        /// <summary>
        /// Output the message to the console.
        /// </summary>
        /// <param name="fileNames"></param>
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
            ReturnNewFileOperation();
            ComReleaser<IShellItem> siSrc = FileOperation.CreateShellItem(src);
            ComReleaser<IShellItem> siDest = FileOperation.CreateShellItem(dest);
            _currentFileOperation.CopyItem(siSrc.Item, siDest.Item, destName, null);
        }

        public void MoveItem(string src, string dest, string destName)
        {
            Console.WriteLine("Add MoveItem");
            ReturnNewFileOperation();
            ComReleaser<IShellItem> siSrc = FileOperation.CreateShellItem(src);
            ComReleaser<IShellItem> siDest = FileOperation.CreateShellItem(dest);
            _currentFileOperation.MoveItem(siSrc.Item, siDest.Item, destName, null);
        }

        public void DeleteItem(string src)
        {
            Console.WriteLine("Add DeleteItem");
            ReturnNewFileOperation();
            ComReleaser<IShellItem> siSrc = FileOperation.CreateShellItem(src);
            _currentFileOperation.DeleteItem(siSrc.Item, null);
        }

        public void RenameItem(string src, string dest)
        {
            Console.WriteLine("Add RenameItem");
            ReturnNewFileOperation();
            ComReleaser<IShellItem> siSrc = FileOperation.CreateShellItem(src);
            _currentFileOperation.RenameItem(siSrc.Item, dest, null);
        }

        public void PerformOperations()
        {
            Console.WriteLine("PerformOperation");
            Task.Run(() =>
            {
                _performDone = true;
                _currentFileOperation.PerformOperations();
            });
        }

        public uint NewItem(string destFolder, FileAttributes dwFileAttributes, string filename, string templateName)
        {
            Console.WriteLine("Add NewItem");
            ReturnNewFileOperation();
            ComReleaser<IShellItem> siDestFolder = FileOperation.CreateShellItem(destFolder);
            _currentFileOperation.NewItem(siDestFolder.Item, dwFileAttributes, filename, templateName, null);
            return 0;
        }

        private void ReturnNewFileOperation()
        {
            if (_performDone)
            {
                _currentFileOperation = (IFileOperation)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("3ad05575-8857-4850-9277-11b85bdb8e09")));
                _performDone = false;
            }
        }
    }
}
