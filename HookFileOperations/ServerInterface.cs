using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Explorip.HookFileOperations.FilesOperations.Interfaces;

namespace Explorip.HookFileOperations
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    public class ServerInterface : MarshalByRefObject
    {
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
            Console.WriteLine("The target process has reported an error:\r\n" + e.ToString());
        }

        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
            // Just to test if server still exist
        }

        public void CopyItem(IShellItem src, IShellItem dest, string destName, IFileOperationProgressSink options)
        {
            Console.WriteLine("Launch CopyItem");
            Task.Run(() =>
            {
                IFileOperation fileOperation = ReturnNewFileOperation();
                fileOperation.CopyItem(src, dest, destName, null);
                fileOperation.PerformOperations();
            });
        }

        public void CopyItems(IntPtr listSrc, IntPtr dest)
        {
            Console.WriteLine("Launch CopyItems");
            Task.Run(() =>
            {
                IFileOperation fileOperation = ReturnNewFileOperation();
                fileOperation.CopyItems((IShellItemArray)Marshal.GetObjectForIUnknown(listSrc), (IShellItem)Marshal.GetObjectForIUnknown(dest));
                fileOperation.PerformOperations();
            });
        }

        private IFileOperation ReturnNewFileOperation()
        {
            return (IFileOperation)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("3ad05575-8857-4850-9277-11b85bdb8e09")));
        }
    }
}
