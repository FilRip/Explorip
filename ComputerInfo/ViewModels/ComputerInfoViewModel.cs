using System;
using System.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using ComputerInfo.Helpers;

namespace ComputerInfo.ViewModels;

public partial class ComputerInfoViewModel : ObservableObject, IDisposable
{
    private readonly Timer _timer;
    [ObservableProperty()]
    private double _freeRam;
    [ObservableProperty()]
    private double _cpuUsage;
    private bool disposedValue;

    public ComputerInfoViewModel() : base()
    {
        _timer = new(RefreshInfo, null, 1000, 1000);
    }

    private void RefreshInfo(object userState)
    {
        CpuUsage = Math.Round(ComputerInfoCpu.PercentCpuUsed, 2);
        FreeRam = Math.Round((double)ComputerInfoMemory.TotalFree / 1024, 2);
    }

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
                _timer?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
