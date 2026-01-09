using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;

using ManagedShell.Interop;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemWindowEmbeddedViewModel(TabItemWindowEmbedded control) : TabItemExploripViewModel(), IDisposable
{
    private readonly TabItemWindowEmbedded _currentControl = control;
    [ObservableProperty()]
    private bool _enabled = false;
    [ObservableProperty()]
    private bool _alreadyWaiting = false;
    private CancellationTokenSource _cancelWaiting;
    private bool disposedValue;

    [RelayCommand()]
    public void EmbedWindow()
    {
        if (AlreadyWaiting)
        {
            _cancelWaiting?.Cancel();
            return;
        }
        _cancelWaiting?.Dispose();
        _cancelWaiting = new CancellationTokenSource();
        Task.Run(WaitForWindow, _cancelWaiting.Token);
    }

    private async Task WaitForWindow()
    {
        while (!_cancelWaiting.IsCancellationRequested && !disposedValue)
        {
            AlreadyWaiting = true;
            IntPtr currentWindow = NativeMethods.GetForegroundWindow();
            NativeMethods.GetWindowThreadProcessId(currentWindow, out uint pid);
            if (pid != Process.GetCurrentProcess().Id)
            {
                _currentControl.EmbeddedWindow.InitRedirectWindow(currentWindow);
                Enabled = true;
                break;
            }
            await Task.Delay(10);
        }
        AlreadyWaiting = false;
    }

    [RelayCommand()]
    public void UnEmbeddedWindow()
    {
        if (Enabled)
            _currentControl.EmbeddedWindow.Detach();
        Enabled = false;
    }

    #region IDisposable

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
                if (Enabled)
                    UnEmbeddedWindow();
                _cancelWaiting?.Cancel();
                _cancelWaiting?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
