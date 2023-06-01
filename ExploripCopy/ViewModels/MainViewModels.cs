using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Explorip.HookFileOperations;
using Explorip.HookFileOperations.Models;

using ManagedShell.Interop;

namespace ExploripCopy.ViewModels
{
    internal class MainViewModels : ViewModelBase
    {
        private static MainViewModels _instance;
        private readonly Thread _mainThread;
        private readonly object _lockOperation;

        public static MainViewModels Instance
        {
            get { return _instance ??= new MainViewModels(); }
        }

        internal MainViewModels() : base()
        {
            _lockOperation = new object();
            _listWait = new();
            _mainThread = new Thread(new ThreadStart(ThreadFileOpWaiting));
            _mainThread.Start();
        }

        private bool _isMaximized;
        public bool WindowMaximized
        {
            get { return _isMaximized; }
            set
            {
                if (value != _isMaximized)
                {
                    _isMaximized = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentFile;
        public string CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                OnPropertyChanged();
            }
        }

        private string _globalReport;
        public string GlobalReport
        {
            get { return _globalReport; }
            set
            {
                _globalReport = value;
                OnPropertyChanged();
            }
        }

        private double _reportProgress;
        public double CurrentProgress
        {
            get { return _reportProgress; }
            set
            {
                _reportProgress = value;
                OnPropertyChanged();
            }
        }

        private double _globalProgress;
        public double GlobalProgress
        {
            get { return _globalProgress; }
            set
            {
                _globalProgress = value;
                OnPropertyChanged();
            }
        }

        private List<OneFileOperation> _listWait;
        public List<OneFileOperation> ListWaiting
        {
            get { return _listWait; }
            set
            {
                _listWait = value;
                OnPropertyChanged();
            }
        }
        public void ForceUpdateWaitingList()
        {
            OnPropertyChanged(nameof(ListWaiting));
        }

        private Exception _lastError;
        public Exception GetLastError
        {
            get { return _lastError; }
            set
            {
                _lastError = value;
                OnPropertyChanged();
            }
        }

        public int _lastSpeed;
        public int LastSpeed
        {
            get { return _lastSpeed; }
            set
            {
                _lastSpeed = value;
                OnPropertyChanged();
            }
        }

        private void Callback_Operation(string currentFile, long fullSize, long remainingSize, int speed)
        {
            CurrentFile = currentFile;
            _lastSpeed = speed;
            long diff = fullSize - remainingSize;
            if (diff < 0)
                diff = fullSize;
            CurrentProgress = diff / (double)fullSize * 100; // Convert in percent
        }

        private void FinishCurrent()
        {
            _listWait.RemoveAt(0);
            _currentOperation = null;
            OnPropertyChanged(nameof(ListWaiting));
        }

        private OneFileOperation _currentOperation;
        private void Treatment(OneFileOperation operation)
        {
            _currentOperation = operation;
            if (operation.FileOperation == EFileOperation.Copy || operation.FileOperation == EFileOperation.Move)
            {
                DirectoryInfo srcDir, destDir;
                srcDir = new DirectoryInfo(operation.Source).Parent;
                destDir = new DirectoryInfo(operation.Destination);
                bool isDirectory = Directory.Exists(operation.Source);
                if (operation.FileOperation == EFileOperation.Copy)
                {
                    if (isDirectory)
                        _lastError = Helpers.CopyHelper.CopyDirectory(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                    else
                        _lastError = Helpers.CopyHelper.CopyFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                }
                else if (srcDir.FullName != destDir.FullName) // If Move in same folder, then nothing to do
                {
                    if (isDirectory)
                        _lastError = Helpers.CopyHelper.MoveDirectory(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
                    else
                        _lastError = Helpers.CopyHelper.MoveFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
                }
                FinishCurrent();
            }
            else
            {
                CurrentFile = operation.Source ?? operation.NewName;
                Task.Run(() =>
                {
                    FileOperation fo = new(NativeMethods.GetDesktopWindow());
                    fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.Interfaces.EFileOperation.FOF_SILENT);
                    switch (operation.FileOperation)
                    {
                        case EFileOperation.Delete:
                            if (operation.ForceDeleteNoRecycled)
                                fo.DeleteItem(operation.Source);
                            else
                            {
                                fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.Interfaces.EFileOperation.FOFX_RECYCLEONDELETE);
                                fo.DeleteItem(operation.Source);
                            }
                            break;
                        case EFileOperation.Rename:
                            fo.RenameItem(operation.Source, operation.NewName);
                            break;
                        case EFileOperation.Create:
                            fo.NewItem(operation.Destination, operation.NewName, operation.Attributes);
                            break;
                    }
                    try
                    {
                        fo.PerformOperations();
                    }
                    catch (Exception ex)
                    {
                        _lastError = ex;
                    }
                    finally
                    {
                        fo?.Dispose();
                    }
                    FinishCurrent();
                });
            }
        }

        private void ThreadFileOpWaiting()
        {
            while (true)
            {
                try
                {
                    if (_currentOperation == null && _listWait.Count > 0 && _lastError == null)
                    {
                        lock (_lockOperation)
                        {
                            Treatment(_listWait[0]);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (ThreadAbortException) { break; }
                catch (Exception) { /* Ignore errors */ }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _mainThread?.Abort();
        }
    }
}
