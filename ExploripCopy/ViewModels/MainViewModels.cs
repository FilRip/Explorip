using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.HookFileOperations;
using Explorip.HookFileOperations.Models;

using ExploripCopy.Constants;
using ExploripCopy.Helpers;
using ExploripCopy.Models;

using ManagedShell.Interop;

namespace ExploripCopy.ViewModels
{
    internal class MainViewModels : ViewModelBase
    {
        private static MainViewModels _instance;
        private readonly Thread _mainThread;
        private readonly object _lockOperation;
        private readonly Stopwatch _chronoSpeed;

        public event EventHandler ForceRefreshList;

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
            _chronoSpeed = new Stopwatch();
            CmdRemoveLine = new RelayCommand(new Action<object>((param) => RemoveLine()));
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
                _currentFile = value.Replace("_", "__");
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
                OnPropertyChanged(nameof(ColorGlobalReport));
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

        private double _maxGlobalProgress;
        public double MaxGlobalProgress
        {
            get { return _maxGlobalProgress; }
            set
            {
                if (_maxGlobalProgress != value)
                {
                    _maxGlobalProgress = value;
                    OnPropertyChanged();
                }
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
                OnPropertyChanged(nameof(TxtGlobalReport));
            }
        }

        public string TxtGlobalReport
        {
            get
            {
                return CopyHelper.SizeInText(_globalProgress, Localization.TOTAL);
            }
        }

        public SolidColorBrush ColorGlobalReport
        {
            get
            {
                if (_lastError == null)
                    return Constants.Colors.ForegroundColorBrush;
                else
                    return Brushes.Red;
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
            ForceRefreshList?.Invoke(this, EventArgs.Empty);
        }

        public OneFileOperation SelectedLine { get; set; }

        private Exception _lastError;
        public Exception GetLastError
        {
            get { return _lastError; }
            set
            {
                _lastError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorGlobalReport));
            }
        }

        public double _lastSpeed;
        public string TxtCurrentSpeed
        {
            get
            {
                return CopyHelper.SizeInText(_lastSpeed, Localization.SPEED_COPY);
            }
        }

        public double LastSpeed
        {
            get { return _lastSpeed; }
            set
            {
                if (_lastSpeed != value)
                {
                    _lastSpeed = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TxtCurrentSpeed));
                }
            }
        }

        private long _currentSpeed;
        private void Callback_Operation(string currentFile, long fullSize, long remainingSize, long nbBytesRead)
        {
            CurrentFile = currentFile;
            _currentSpeed += nbBytesRead;
            if (_chronoSpeed.IsRunning && _chronoSpeed.ElapsedMilliseconds >= 1000)
            {
                LastSpeed = _currentSpeed;
                _chronoSpeed.Restart();
                _currentSpeed = 0;
            }
            long diff = fullSize - remainingSize;
            if (diff < 0)
                diff = fullSize;
            CurrentProgress = diff / (double)fullSize * 100; // Convert in percent
            GlobalProgress += nbBytesRead;
        }

        private void FinishCurrent()
        {
            NotifyIconViewModel.Instance.SystrayControl.HideBalloonTip();
            if (_lastError == null)
            {
                _listWait.RemoveAt(0);
                _currentOperation = null;
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.FINISH, GlobalReport, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                GlobalReport = Localization.FINISH;
            }
            else
            {
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.ERROR, GlobalReport, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                GlobalReport = _lastError.Message;
            }
            ForceUpdateWaitingList();
            CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
            NotifyIconViewModel.Instance.SetSystrayIcon(false);
            _chronoSpeed.Stop();
        }

        private void UpdateGlobalReport()
        {
            StringBuilder sb = new();
            switch (_currentOperation.FileOperation)
            {
                case EFileOperation.Copy:
                    sb.Append(Localization.COPY_OF_FILESYSTEM);
                    break;
                case EFileOperation.Move:
                    sb.Append(Localization.MOVE_OF_FILESYSTEM);
                    break;
                case EFileOperation.Delete:
                    sb.Append(Localization.DELETE_OF_FILESYSTEM);
                    break;
                case EFileOperation.Rename:
                    sb.Append(Localization.RENAME_OF_FILESYSTEM);
                    break;
                case EFileOperation.Create:
                    sb.Append(Localization.CREATE_OF_FILESYSTEM);
                    break;
            }
            sb = sb.Replace("%s", _currentOperation.Source);
            GlobalReport = sb.ToString();
            NotifyIconViewModel.Instance.SystrayControl.HideBalloonTip();
            NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.IN_PROGRESS, GlobalReport, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }
        private OneFileOperation _currentOperation;
        private void Treatment(OneFileOperation operation)
        {
            _currentOperation = operation;
            NotifyIconViewModel.Instance.SetSystrayIcon(true);
            GlobalProgress = 0;
            UpdateGlobalReport();
            _chronoSpeed.Restart();
            _lastSpeed = 0;
            if (operation.FileOperation == EFileOperation.Copy || operation.FileOperation == EFileOperation.Move)
            {
                CurrentFile = operation.Source;
                Task.Run(() =>
                {
                    try
                    {
                        DirectoryInfo srcDir, destDir;
                        srcDir = new DirectoryInfo(operation.Source).Parent;
                        destDir = new DirectoryInfo(operation.Destination);
                        bool isDirectory = Directory.Exists(operation.Source);
                        if (operation.FileOperation == EFileOperation.Copy)
                        {
                            if (isDirectory)
                            {
                                GlobalReport = Localization.CALCUL;
                                MaxGlobalProgress = CopyHelper.TotalSizeDirectory(operation.Source);
                                UpdateGlobalReport();
                                GetLastError = CopyHelper.CopyDirectory(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                            }
                            else
                            {
                                MaxGlobalProgress = new FileInfo(operation.Source).Length;
                                GetLastError = CopyHelper.CopyFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                            }
                        }
                        else if (srcDir.FullName != destDir.FullName) // If Move in same folder, then nothing to do
                        {
                            if (Path.GetPathRoot(Path.GetFullPath(srcDir.FullName)).CompareTo(Path.GetPathRoot(Path.GetFullPath(destDir.FullName))) == 0)
                            {
                                // If move in same disk, just change location in file system table
                                try
                                {
                                    if (isDirectory)
                                        Directory.Move(operation.Source, destDir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(operation.Source));
                                    else
                                        File.Move(operation.Source, destDir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(operation.Source));
                                }
                                catch (Exception ex)
                                {
                                    GetLastError = ex;
                                }
                            }
                            else
                            {
                                // Finally, else, we must Copy/Delete
                                if (isDirectory)
                                {
                                    GlobalReport = Localization.CALCUL;
                                    MaxGlobalProgress = CopyHelper.TotalSizeDirectory(operation.Source);
                                    UpdateGlobalReport();
                                    GetLastError = CopyHelper.MoveDirectory(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
                                }
                                else
                                {
                                    MaxGlobalProgress = new FileInfo(operation.Source).Length;
                                    GetLastError = CopyHelper.MoveFile(operation.Source, operation.Destination, CallbackRefresh: Callback_Operation);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLastError = ex;
                    }
                    finally
                    {
                        FinishCurrent();
                    }
                });
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
                        GetLastError = ex;
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

        public ICommand CmdRemoveLine { get; private set; }

        private void RemoveLine()
        {
            try
            {
                ListWaiting.Remove(SelectedLine);
                ForceUpdateWaitingList();
            }
            catch (Exception) { /* Ignore errors */ }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _mainThread?.Abort();
        }
    }
}
