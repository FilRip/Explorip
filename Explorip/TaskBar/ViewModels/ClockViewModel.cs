﻿using System;
using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Utilities;

using ManagedShell.Common.Helpers;

using Microsoft.WindowsAPICodePack.Shell.Constants;

namespace Explorip.TaskBar.ViewModels;

public partial class ClockViewModel : ObservableObject
{
    [ObservableProperty()]
    private string _clockText, _clockTip;
    [ObservableProperty()]
    private Visibility _clockVisibility;

    private readonly DispatcherTimer clock = new(DispatcherPriority.Background);
    private readonly DispatcherTimer singleClick = new(DispatcherPriority.Input);
    private Dispatcher _dispatcher;

    public ClockViewModel() : base()
    {
        if (Settings.Instance.ShowClock)
        {
            StartClock();
        }

        Settings.Instance.PropertyChanged += Settings_PropertyChanged;
        Microsoft.Win32.SystemEvents.TimeChanged += TimeChanged;
    }

    public void SetDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void StartClock()
    {
        SetTime();

        clock.Interval = TimeSpan.FromMilliseconds(500);
        clock.Tick += Clock_Tick;
        clock.Start();

        singleClick.Interval = TimeSpan.FromMilliseconds(SystemInformations.DoubleClickTime);
        singleClick.Tick += SingleClick_Tick;

        ClockVisibility = Visibility.Visible;
    }

    private void StopClock()
    {
        clock.Stop();

        ClockVisibility = Visibility.Collapsed;
    }

    private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ShowClock")
        {
            if (Settings.Instance.ShowClock)
            {
                StartClock();
            }
            else
            {
                StopClock();
            }
        }
    }

    private void Clock_Tick(object sender, EventArgs args)
    {
        SetTime();
    }

    public void SingleClickStop()
    {
        singleClick.Stop();
    }

    private void SingleClick_Tick(object sender, EventArgs args)
    {
        // Windows 11 single-click action
        // A double-click will cancel the timer so that this doesn't run

        singleClick.Stop();
        ShellHelper.ShowNotificationCenter();
    }

    private void TimeChanged(object sender, EventArgs e)
    {
        TimeZoneInfo.ClearCachedData();
    }

    private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
    {
        Microsoft.Win32.SystemEvents.TimeChanged -= TimeChanged;
    }

    private void SetTime()
    {
        DateTime now = DateTime.Now;

        ClockText = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(now.DayOfWeek) + Environment.NewLine + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + Environment.NewLine + now.Day.ToString("00") + "/" + now.Month.ToString("00") + now.Year.ToString("00");
        ClockTip = now.ToLongDateString();
    }
}
