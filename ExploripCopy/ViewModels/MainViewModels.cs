using System;
using System.Collections.Generic;
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

        public static MainViewModels Instance
        {
            get { return _instance ??= new MainViewModels(); }
        }

        internal MainViewModels() : base()
        {
            _listWait = new();
            _mainThread = new Thread(new ThreadStart(ThreadFileOpWaiting));
            //_mainThread.Start();
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

        private void Callback_Operation(long fullSize, long remainingSize, int speed)
        {
            _lastSpeed = speed;
            long diff = fullSize - remainingSize;
            if (diff < 0)
                diff = fullSize;
            CurrentProgress = diff / fullSize * 100; // Convert in percent
        }

        private OneFileOperation _currentOperation;
        private void Treatment(OneFileOperation operation)
        {
            _currentOperation = operation;
            if (operation.FileOperation == EFileOperation.Copy || operation.FileOperation == EFileOperation.Move)
            {
                if (operation.FileOperation == EFileOperation.Copy)
                    _lastError = Helpers.CopyHelper.CopyFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
                else
                    _lastError = Helpers.CopyHelper.MoveFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
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
                        _currentOperation = null;
                    }
                    catch (Exception ex)
                    {
                        _lastError = ex;
                    }
                    finally
                    {
                        fo.Dispose();
                    }
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
                        Treatment(_listWait[0]);
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
