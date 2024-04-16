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

using Explorip.HookFileOperations;
using Explorip.HookFileOperations.Models;

using ExploripCopy.Constants;
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

    [ObservableProperty()]
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
                return Constants.Colors.ForegroundColorBrush;
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
        if (_lastError == null)
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
        if (operation.FileOperation == EFileOperation.Copy || operation.FileOperation == EFileOperation.Move)
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
                    fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.Interfaces.EFileOperation.FOF_SILENT);
                    switch (operation.FileOperation)
                    {
                        case EFileOperation.Delete:
                            if (operation.ForceDeleteNoRecycled || !ExploripSharedCopy.WinAPI.Shell32.RecycledEnabledOnDrive(operation.Source.Substring(0, 2)))
                            {
                                if (CopyHelper.ChoiceOnCollision == EChoiceFileOperation.ConfirmDelete)
                                    fo.ChangeOperationFlags(fo.CurrentFileOperationFlags | Explorip.HookFileOperations.FilesOperations.Interfaces.EFileOperation.FOF_NOCONFIRMATION);
                                else
                                    CopyHelper.ChoiceOnCollision = EChoiceFileOperation.ConfirmDelete;
                                fo.DeleteItem(operation.Source);
                            }
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
                            if (operation.Attributes.HasFlag(FileAttributes.Directory))
                            {
                                static void SetConstants(InputBoxWindow win)
                                {
                                    win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                                    win.Title = Localization.CREATE_FOLDER;
                                    win.Icon = Icons.Folder;
                                    win.Background = Constants.Colors.BackgroundColorBrush;
                                    win.Foreground = Constants.Colors.ForegroundColorBrush;
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
                                    win.Background = Constants.Colors.BackgroundColorBrush;
                                    win.Foreground = Constants.Colors.ForegroundColorBrush;
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
                        resetChoice = ListWaiting[0].ResetChoice;
                        Treatment(ListWaiting[0]);
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
