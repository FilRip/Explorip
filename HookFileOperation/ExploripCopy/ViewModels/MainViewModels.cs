using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.HookFileOperations.FilesOperations;
using Explorip.HookFileOperations.Models;

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
    public event EventHandler ForceRefreshList;
    private Thread _currentThread;

    public static MainViewModels Instance
    {
        get { return _instance ??= new MainViewModels(); }
    }

    internal MainViewModels() : base()
    {
        _lockOperation = new object();
        _listWaiting = [];
        _mainThread = new Thread(new ThreadStart(ThreadFileOpWaiting));
        _mainThread.Start();
        _chronoSpeed = new Stopwatch();
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
    private double _maxGlobalProgress;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TxtGlobalReport))]
    private double _globalProgress;

    public string TxtGlobalReport
    {
        get
        {
            return CopyHelper.SizeInText(GlobalProgress, Localization.TOTAL);
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

    public void ForceUpdateWaitingList()
    {
        OnPropertyChanged(nameof(ListWaiting));
        ForceRefreshList?.Invoke(this, EventArgs.Empty);
    }

    public void AddOperations(List<OneFileOperation> list)
    {
        lock (_lockOperation)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (OneFileOperation op in list)
                    ListWaiting.Add(op);
            });
            ForceUpdateWaitingList();
        }
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
        }
    }

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TxtCurrentSpeed))]
    private double _lastSpeed;

    public string TxtCurrentSpeed
    {
        get
        {
            return CopyHelper.SizeInText(LastSpeed, Localization.SPEED_COPY);
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
    }

    public string CurrentProgressPercent
    {
        get { return Math.Round(CurrentProgress).ToString() + "%"; }
    }

    private void FinishCurrent()
    {
        if (ListWaiting.Count == 1)
            NotifyIconViewModel.Instance.SetSystrayIcon(false);
        if (Settings.ShowBalloon)
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
            if (Settings.ShowBalloon)
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.FINISH, GlobalReport, BalloonIcon.Info);
            GlobalReport = Localization.FINISH;
        }
        else
        {
            if (Settings.ShowBalloon)
                NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.ERROR, GlobalReport, BalloonIcon.Error);
            else
                MainWindow.Instance.ShowWindow();
            GlobalReport = _lastError.Message;
        }
        ForceUpdateWaitingList();
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
        sb = sb.Replace("%s", Path.GetFileName(_currentOperation.Source));
        GlobalReport = sb.ToString();
        if (Settings.ShowBalloon)
        {
            NotifyIconViewModel.Instance.SystrayControl.HideBalloonTip();
            NotifyIconViewModel.Instance.SystrayControl.ShowBalloonTip(Localization.IN_PROGRESS, GlobalReport, BalloonIcon.Info);
        }
    }
    private OneFileOperation _currentOperation;
    private OneFileOperation _nextOperation;
    private bool disposedValue;

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
                            GetLastError = CopyHelper.CopyDirectory(operation.Source, operation.Destination, operation.CurrentOffset, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
                        }
                        else
                        {
                            MaxGlobalProgress = new FileInfo(operation.Source).Length;
                            GetLastError = CopyHelper.CopyFile(operation.Source, operation.Destination, operation.CurrentOffset, CallbackRefresh: Callback_Operation, renameOnCollision: (srcDir.FullName == destDir.FullName));
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
                                GetLastError = CopyHelper.MoveDirectory(operation.Source, operation.Destination, operation.CurrentOffset, CallbackRefresh: Callback_Operation);
                            }
                            else
                            {
                                MaxGlobalProgress = new FileInfo(operation.Source).Length;
                                GetLastError = CopyHelper.MoveFile(operation.Source, operation.Destination, operation.CurrentOffset, CallbackRefresh: Callback_Operation);
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
                                static void SetConstants(CreateShortcutWindow win)
                                {
                                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                                    win.Title = Localization.CREATE_SHORTCUT;
                                    win.Icon = Icons.Shortcut;
                                    win.Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
                                    win.Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
                                    win.SetQuestions(Localization.CREATE_SHORTCUT_Q1, Localization.CREATE_SHORTCUT_Q2);
                                    win.SetOk(Localization.CONTINUE.Replace("_", ""), Icons.OkImage);
                                    win.SetCancel(Localization.CANCEL.Replace("_", ""), Icons.CancelImage);
                                    win.SetBrowse(Localization.BROWSE.Replace("_", ""));
                                }

                                ExploripSharedCopy.Helpers.CreateOperations.CreateShortcut(operation.Destination, operation.NewName, SetConstants);
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
                    lock (_lockOperation)
                    {
                        _nextOperation ??= ListWaiting[0];
                        resetChoice = _nextOperation.ResetChoice;
                        Treatment(_nextOperation);
                        _nextOperation = null;
                    }
                }
                else
                {
                    if (ListWaiting.Count == 0 || resetChoice)
                        CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
                }
                Thread.Sleep(10);
            }
            catch (ThreadAbortException) { break; }
            catch (Exception) { /* Ignore errors */ }
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
            ForceUpdateWaitingList();
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

    [RelayCommand()]
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
            GetLastError = null;
        }
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
}
