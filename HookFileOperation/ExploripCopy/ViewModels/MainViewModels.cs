using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.HookFileOperations.FilesOperations;
using Explorip.HookFileOperations.Models;

using ExploripConfig.Configuration;

using ExploripCopy.Constants;
using ExploripCopy.GUI;
using ExploripCopy.Helpers;
using ExploripCopy.Models;

using ExploripSharedCopy.Controls;

using Hardcodet.Wpf.TaskbarNotification;

using ManagedShell.Interop;

namespace ExploripCopy.ViewModels;

public partial class MainViewModels : ObservableObject, IDisposable
{
    private static MainViewModels _instance;
    private readonly Thread _mainThread;
    private readonly object _lockOperation;
    private readonly Stopwatch _chronoSpeed;
    private Thread _currentThread;
    private OneFileOperation _currentOperation;
    private OneFileOperation _nextOperation;
    private bool _forceNext;

    public static MainViewModels Instance
    {
        get { return _instance ??= new MainViewModels(); }
    }

    internal MainViewModels() : base()
    {
        _autoStartOperation = ExploripCopyConfig.AutoStartOperation;
        _lockOperation = new object();
        _listWaiting = [];
        _listWaiting.CollectionChanged += ListWaiting_CollectionChanged;
        _mainThread = new Thread(new ThreadStart(ThreadFileOpWaiting));
        _mainThread.Start();
        _chronoSpeed = new Stopwatch();
    }

    private void ListWaiting_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            MaxGlobalOperations = ListWaiting.Sum(op => (double)op.Size);
        }
    }

    [ObservableProperty()]
    private bool _windowMaximized;

    [ObservableProperty()]
    private string _currentFile;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ColorGlobalReport))]
    private string _globalReport;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(CurrentProgressPercent))]
    private double _currentProgress;

    [ObservableProperty()]
    private double _maxGlobalProgress, _maxGlobalOperations;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TxtGlobalReport))]
    private double _globalProgress;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TxtGlobalOperations))]
    private double _globalOperations;

    [ObservableProperty()]
    private bool _autoStartOperation;

    public string TxtGlobalReport
    {
        get
        {
            return ExtensionsDirectory.SizeInText(GlobalProgress, Localization.TOTAL);
        }
    }

    public string TxtGlobalTimeRemaining
    {
        get
        {
            try
            {
                double totalRemainingByte = MaxGlobalOperations - GlobalOperations;
                double totalReaminingSeconds = totalRemainingByte / LastSpeed;
                TimeSpan ts = TimeSpan.FromSeconds(totalReaminingSeconds);
                StringBuilder sb = new();
                if (ts.Hours > 1)
                    sb.Append(Localization.TIME_REMAINING_HOURS);
                else if (ts.Hours == 1)
                    sb.Append(Localization.TIME_REMAINING_HOUR);
                if (sb.Length > 0)
                    sb.Append(", ");
                if (ts.Minutes > 1)
                    sb.Append(Localization.TIME_REMAINING_MINUTES_SECONDS);
                else if (ts.Minutes == 1)
                    sb.Append(Localization.TIME_REMAINING_MINUTE_SECONDS);
                else
                    sb.Append(Localization.TIME_REMAINING_SECONDS);
                return sb.ToString().Replace("%h", ts.Hours.ToString()).Replace("%m", ts.Minutes.ToString()).Replace("%s", ts.Seconds.ToString());
            }
            catch (Exception) { /* Ignore errors */ }
            return string.Empty;
        }
    }

    public string TxtGlobalOperations
    {
        get
        {
            return ExtensionsDirectory.SizeInText(GlobalOperations, Localization.TOTAL);
        }
    }

    public SolidColorBrush ColorGlobalReport
    {
        get
        {
            if (_lastError == null)
                return ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
            else
                return Brushes.Red;
        }
    }

    [ObservableProperty()]
    private ObservableCollection<OneFileOperation> _listWaiting;

    public void AddOperations(List<OneFileOperation> list)
    {
        lock (_lockOperation)
        {
            Task.Run(() =>
            {
                foreach (OneFileOperation op in list)
                {
                    if (op.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Copy || op.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Move)
                    {
                        if (op.Attributes == 0)
                            op.Attributes = File.GetAttributes(op.Source);
                        // Calculate size
                        if (op.IsDirectory)
                            op.Size = ExtensionsDirectory.DirectorySize(op.Source);
                        else
                            op.Size = (ulong)(new FileInfo(op.Source)?.Length ?? 0);
                    }

                    if (ExploripCopyConfig.PriorityToLowerOperations &&
                        (op.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Create || op.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Delete || op.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Rename))
                    {
                        AddOrInsertOp(op, 0);
                    }
                    else
                        AddOrInsertOp(op);
                }
            });
        }
    }

    private void AddOrInsertOp(OneFileOperation op, int pos = -1)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            if (pos >= 0)
                ListWaiting.Insert(pos, op);
            else
                ListWaiting.Add(op);
        });
    }

    public List<OneFileOperation> SelectedLines { get; set; } = [];

    private Exception _lastError;
    public Exception GetLastError
    {
        get { return _lastError; }
        set
        {
            if (value is ThreadAbortException)
                _lastError = null;
            else
                _lastError = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ColorGlobalReport));
            if (value != null)
                MainWindow.Instance.ShowWindow();
        }
    }

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TxtCurrentSpeed))]
    private double _lastSpeed;

    partial void OnLastSpeedChanged(double value)
    {
        if (value > 0)
            OnPropertyChanged(nameof(TxtGlobalTimeRemaining));
    }

    public string TxtCurrentSpeed
    {
        get
        {
            return ExtensionsDirectory.SizeInText(LastSpeed, Localization.SPEED_COPY);
        }
    }

    private long _currentSpeed;
    private void Callback_Operation(string currentFile, long fullSize, long remainingSize, long nbBytesRead)
    {
        _currentOperation.CurrentOffset += nbBytesRead;
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
        GlobalOperations = ListWaiting.Sum(op => op.CurrentOffset);
    }

    public string CurrentProgressPercent
    {
        get { return Math.Round(CurrentProgress).ToString() + "%"; }
    }

    private void FinishCurrent()
    {
        if (ListWaiting.Count == 1)
            NotifyIconViewModel.Instance.SetSystrayIcon(false);
        if (ExploripCopyConfig.NotificationOnEachOperation)
            NotifyIconViewModel.Instance.SystrayControl.HideBalloonTip();
        lock (_lockOperation)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (ListWaiting.Count > 0)
                {
                    SelectedLines.Remove(_currentOperation);
                    ListWaiting.Remove(_currentOperation);
                }
            });
        }
        if (_lastError == null || _lastError.Message == Localization.STOP)
        {
            _currentOperation = null;
            if (ExploripCopyConfig.NotificationOnEachOperation)
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.FINISH, GlobalReport, BalloonIcon.Info);
            GlobalReport = Localization.FINISH;
        }
        else
        {
            if (ExploripCopyConfig.NotificationOnEachOperation)
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.ERROR, GlobalReport, BalloonIcon.Error);
            else
                MainWindow.Instance.ShowWindow();
            GlobalReport = _lastError.Message;
        }
        _chronoSpeed.Stop();
    }

    private void UpdateGlobalReport()
    {
        StringBuilder sb = new();
        switch (_currentOperation.FileOperation)
        {
            case Explorip.HookFileOperations.Models.EFileOperation.Copy:
                sb.Append(Localization.COPY_OF_FILESYSTEM);
                break;
            case Explorip.HookFileOperations.Models.EFileOperation.Move:
                sb.Append(Localization.MOVE_OF_FILESYSTEM);
                break;
            case Explorip.HookFileOperations.Models.EFileOperation.Delete:
                sb.Append(Localization.DELETE_OF_FILESYSTEM);
                break;
            case Explorip.HookFileOperations.Models.EFileOperation.Rename:
                sb.Append(Localization.RENAME_OF_FILESYSTEM);
                break;
            case Explorip.HookFileOperations.Models.EFileOperation.Create:
                sb.Append(Localization.CREATE_OF_FILESYSTEM);
                break;
        }
        sb = sb.Replace("%s2", _currentOperation.NewName);
        sb = sb.Replace("%s", Path.GetFileName(_currentOperation.DisplaySource));
        GlobalReport = sb.ToString();
        if (ExploripCopyConfig.NotificationOnEachOperation)
        {
            NotifyIconViewModel.Instance.SystrayControl.HideBalloonTip();
            NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.IN_PROGRESS, GlobalReport, BalloonIcon.Info);
        }
    }

    private void Treatment(OneFileOperation operation)
    {
        _currentOperation = operation;
        NotifyIconViewModel.Instance.SetSystrayIcon(true);
        GlobalProgress = 0;
        UpdateGlobalReport();
        _chronoSpeed.Restart();
        LastSpeed = 0;
        if (_currentThread?.IsAlive == true)
            _currentThread.Abort();
        if (operation.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Copy || operation.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Move)
        {
            CurrentFile = operation.Source;
            _currentThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    DirectoryInfo srcDir, destDir;
                    srcDir = new DirectoryInfo(operation.Source).Parent;
                    destDir = new DirectoryInfo(operation.Destination);
                    bool isDirectory = Directory.Exists(operation.Source);
                    if (operation.FileOperation == Explorip.HookFileOperations.Models.EFileOperation.Copy)
                    {
                        if (isDirectory)
                        {
                            GlobalReport = Localization.CALCUL;
                            MaxGlobalProgress = CopyHelper.TotalSizeDirectory(operation.Source);
                            UpdateGlobalReport();
                            GetLastError = CopyHelper.CopyDirectory(operation.Source, operation.Destination, operation.CurrentOffset, (uint)ExploripCopyConfig.MaxBufferSize, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                        }
                        else
                        {
                            MaxGlobalProgress = new FileInfo(operation.Source).Length;
                            GetLastError = CopyHelper.CopyFile(operation.Source, operation.Destination, operation.CurrentOffset, (uint)ExploripCopyConfig.MaxBufferSize, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
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
                                    MoveSameDriveHelper.MoveDirectory(operation.Source, destDir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(operation.NewName), Callback_Operation);
                                else
                                    MoveSameDriveHelper.MoveFile(operation.Source, destDir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(operation.NewName), Callback_Operation);
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
                                GetLastError = CopyHelper.MoveDirectory(operation.Source, operation.Destination, operation.CurrentOffset, (uint)ExploripCopyConfig.MaxBufferSize, CallbackRefresh: Callback_Operation);
                            }
                            else
                            {
                                MaxGlobalProgress = new FileInfo(operation.Source).Length;
                                GetLastError = CopyHelper.MoveFile(operation.Source, operation.Destination, operation.CurrentOffset, (uint)ExploripCopyConfig.MaxBufferSize, CallbackRefresh: Callback_Operation);
                            }
                        }
                    }
                }
                catch (ThreadAbortException) { /* Nothing to do */ }
                catch (Exception ex)
                {
                    GetLastError = ex;
                }
                finally
                {
                    FinishCurrent();
                }
            }));
            _currentThread.Start();
        }
        else
        {
            CurrentFile = operation.Source ?? operation.NewName;
            _currentThread = new Thread(new ThreadStart(() =>
            {
                FileOperation fo = null;
                try
                {
                    fo = new(NativeMethods.GetDesktopWindow());
                    fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_SILENT);
                    switch (operation.FileOperation)
                    {
                        case Explorip.HookFileOperations.Models.EFileOperation.Delete:
                            if (operation.ForceDeleteNoRecycled || !ExploripSharedCopy.WinAPI.Shell32.RecycledEnabledOnDrive(operation.Source.Substring(0, 2)))
                            {
                                if (CopyHelper.ChoiceOnCollision == EChoiceFileOperation.ConfirmDelete)
                                    fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_NOCONFIRMATION);
                                else
                                    CopyHelper.ChoiceOnCollision = EChoiceFileOperation.ConfirmDelete;
                                fo.DeleteItem(operation.Source);
                            }
                            else
                            {
                                fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.EFileOperation.FOFX_RECYCLEONDELETE);
                                fo.DeleteItem(operation.Source);
                            }
                            break;
                        case Explorip.HookFileOperations.Models.EFileOperation.Rename:
                            fo.RenameItem(operation.Source, operation.NewName);
                            break;
                        case Explorip.HookFileOperations.Models.EFileOperation.Create:
                            if (operation.Attributes.HasFlag(FileAttributes.Directory))
                            {
                                static void SetConstants(InputBoxWindow win)
                                {
                                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                                    win.Title = Localization.CREATE_FOLDER;
                                    win.Icon = Icons.Folder;
                                    win.Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
                                    win.Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
                                    win.SetOk(Localization.CONTINUE.Replace("_", ""), Icons.OkImage);
                                    win.SetCancel(Localization.CANCEL.Replace("_", ""), Icons.CancelImage);
                                }

                                ExploripSharedCopy.Helpers.CreateOperations.CreateFolder(operation.Destination, operation.NewName, SetConstants);
                                break;
                            }
                            if (Path.GetExtension(operation.NewName).ToLower() == ".lnk")
                            {
                                ExploripSharedCopy.Helpers.CreateOperations.CreateShortcut(operation.Destination, Path.GetFileNameWithoutExtension(operation.NewName));
                                break;
                            }
                            fo.NewItem(operation.Destination, operation.NewName, operation.Attributes);
                            break;
                    }
                    fo.PerformOperations();
                }
                catch (ThreadAbortException) { /* Nothing to do */ }
                catch (Exception ex)
                {
                    GetLastError = ex;
                }
                finally
                {
                    fo?.Dispose();
                    FinishCurrent();
                }
                FinishCurrent();
            }));
            _currentThread.Start();
        }
    }

    private void ThreadFileOpWaiting()
    {
        bool resetChoice = false;
        while (true)
        {
            try
            {
                if (_currentOperation == null && ListWaiting.Count > 0 && GetLastError == null)
                {
                    if (AutoStartOperation || _forceNext)
                    {
                        _forceNext = false;
                        lock (_lockOperation)
                        {
                            _nextOperation ??= ListWaiting[0];
                            resetChoice = _nextOperation.ResetChoice;
                            Treatment(_nextOperation);
                            _nextOperation = null;
                        }
                    }
                }
                else
                {
                    if (ListWaiting.Count == 0 || resetChoice)
                        CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
                }
            }
            catch (ThreadAbortException) { break; }
            catch (Exception) { /* Ignore errors */ }
            Thread.Sleep(10);
        }
    }

    [RelayCommand()]
    private void RemoveLine()
    {
        try
        {
            lock (_lockOperation)
            {
                if (SelectedLines.Count > 0)
                    foreach (OneFileOperation op in SelectedLines)
                        ListWaiting.Remove(op);
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    [RelayCommand()]
    private void IgnoreCurrent()
    {
        if (_currentThread?.IsAlive == true)
            _currentThread.Abort();
        else if (_lastError != null)
        {
            GetLastError = null;
            _currentOperation = null;
        }
    }

    [RelayCommand()]
    private void DoPause()
    {
        if (CurrentFile != null && _lastError == null)
            CopyHelper.Pause = !CopyHelper.Pause;
    }

    internal void StartNow()
    {
        if (_currentOperation != null)
        {
            if (_currentOperation == SelectedLines[0])
                return;
            CopyHelper.Stop = true;
            while (GetLastError == null)
            {
                Thread.Sleep(10);
            }
            CopyHelper.Stop = false;
        }
        lock (_lockOperation)
        {
            _nextOperation = SelectedLines[0];
            _forceNext = true;
            GetLastError = null;
        }
    }

    public bool OperationInProgress
    {
        get { return _currentOperation != null; }
    }

    partial void OnAutoStartOperationChanged(bool value)
    {
        ExploripCopyConfig.AutoStartOperation = value;
    }

    public void ChangeWindowState(object param)
    {
        WindowMaximized = (param.ToString() == "1");
    }

    [RelayCommand()]
    private void StartNowButton(object param)
    {
        SelectedLines = [(OneFileOperation)param];
        StartNow();
    }

    #region IDisposable

    private bool disposedValue;
    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _mainThread?.Abort();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
