using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ComputerInfo.Helpers;

namespace ComputerInfo.ViewModels;

public partial class ComputerInfoViewModel : ObservableObject, IDisposable
{
    private Timer _timer;

    [ObservableProperty()]
    private int _period;
    [ObservableProperty()]
    private int _precisionCpu;
    [ObservableProperty()]
    private double _freeRam;
    [ObservableProperty()]
    private double _cpuUsage;
    [ObservableProperty()]
    private SolidColorBrush _backgroundColor, _foregroundColor;
    [ObservableProperty()]
    private bool _isPause;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(HorizontalIsChecked), nameof(VerticalIsChecked))]
    private Orientation _currentOrientation;
    [ObservableProperty()]
    private Brush _cpuColor, _ramColor;

    public ComputerInfoViewModel() : base()
    {
        _period = 1000;
        _precisionCpu = 2;
        InitTimer();
        _currentOrientation = Orientation.Horizontal;
    }

    private void InitTimer()
    {
        _timer = new(RefreshInfo, null, Period, Period);
    }

    private void RefreshInfo(object userState)
    {
        CpuUsage = Math.Round(Math.Min(100, ComputerInfoCpu.PercentCpuUsed), PrecisionCpu);
        FreeRam = Math.Round((double)ComputerInfoMemory.TotalFree / 1024, 2);
        if (CpuUsage < 33)
            CpuColor = Brushes.LightGreen;
        else if (CpuUsage < 66)
            CpuColor = Brushes.Yellow;
        else
            CpuColor = Brushes.OrangeRed;
    }

    public bool HorizontalIsChecked
    {
        get { return CurrentOrientation == Orientation.Horizontal; }
    }

    public bool VerticalIsChecked
    {
        get { return CurrentOrientation == Orientation.Vertical; }
    }

    [RelayCommand()]
    private void ChangePeriod(string param)
    {
        if (int.TryParse(param, out int newPeriod))
        {
            Period = newPeriod;
            _timer.Change(Period, Period);
        }
    }

    [RelayCommand()]
    private void ChangePrecisionCpu(string param)
    {
        if (int.TryParse(param, out int newPrecisionCpu))
        {
            PrecisionCpu = newPrecisionCpu;
        }
    }

    [RelayCommand()]
    public void Pause()
    {
        if (_timer == null)
            InitTimer();
        if (IsPause)
            _timer.Change(Period, Period);
        else
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        IsPause = !IsPause;
    }

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
    internal void ChangeColor(SolidColorBrush background, SolidColorBrush foreground, SolidColorBrush accent)
    {
        BackgroundColor = Brushes.Transparent;
        ForegroundColor = foreground;
    }
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé

    [RelayCommand()]
    private void SetOrientation(string param)
    {
        if (param == "H")
            CurrentOrientation = Orientation.Horizontal;
        else
            CurrentOrientation = Orientation.Vertical;
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
                IsPause = true;
                _timer?.Dispose();
                _timer = null;
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
