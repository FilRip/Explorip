using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Structs;

namespace ManagedShell.ShellFolders
{
    public class FileOperationWorker
    {
        private readonly BackgroundWorker _worker = new();

        public FileOperationWorker()
        {
            _worker.DoWork += WorkerDoWork;
        }

        public void PasteFromClipboard(string targetDirectory)
        {
            IDataObject clipFiles = Clipboard.GetDataObject();

            if (clipFiles == null)
            {
                return;
            }

            if (clipFiles.GetDataPresent(DataFormats.FileDrop) && clipFiles.GetData(DataFormats.FileDrop) is string[] files)
            {
                PerformOperation(FileOperation.Copy, files, targetDirectory);
            }
        }

        public void PerformOperation(FileOperation operation, string[] files, string targetDirectory)
        {
            _worker.RunWorkerAsync(new BackgroundFileOperation
            {
                Paths = files,
                Operation = operation,
                TargetPath = targetDirectory
            });
        }

        private void DoOperation(BackgroundFileOperation operation, string path)
        {
            try
            {
                if (!ShellHelper.Exists(path))
                {
                    return;
                }

                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DoDirectoryOperation(operation, path);
                }
                else
                {
                    DoFileOperation(operation, path);
                }
            }
            catch (Exception e)
            {
                ShellLogger.Error($"FileOperationWorker: Unable to perform {operation.Operation} on {path} into {operation.TargetPath}: {e.Message}");
            }
        }

        private void DoDirectoryOperation(BackgroundFileOperation operation, string path)
        {
            if (path == operation.TargetPath)
            {
                return;
            }

            string futureName = Path.Combine(operation.TargetPath, new DirectoryInfo(path).Name);
            if (futureName == path)
            {
                return;
            }

            switch (operation.Operation)
            {
                case FileOperation.Copy:
                    CopyDirectory(path, futureName, true);
                    break;
                case FileOperation.Move:
                    Directory.Move(path, futureName);
                    break;
            }
        }

        private void DoFileOperation(BackgroundFileOperation operation, string path)
        {
            string futureName = Path.Combine(operation.TargetPath, Path.GetFileName(path));
            if (futureName == path)
            {
                return;
            }

            switch (operation.Operation)
            {
                case FileOperation.Copy:
                    File.Copy(path, futureName, true);
                    break;
                case FileOperation.Move:
                    File.Move(path, futureName);
                    break;
            }
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundFileOperation operation = (BackgroundFileOperation)e.Argument;

            foreach (var path in operation.Paths)
            {
                DoOperation(operation, path);
            }
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
