﻿using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExploripConfig.Configuration;

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
    private readonly string[] _dateFormat;

    public ClockViewModel()
    {
        _dateFormat = System.Text.RegularExpressions.Regex.Split(ConfigManager.DateFormat.Trim().Replace("\\r\\n", Environment.NewLine).Replace("\\r", Environment.NewLine), "(<%.*?%>)");
    }

    public void ShowClock()
    {
        StartClock();
        Microsoft.Win32.SystemEvents.TimeChanged += TimeChanged;
    }

    public void HideClock()
    {
        clock.Stop();
        singleClick.Stop();
        Microsoft.Win32.SystemEvents.TimeChanged -= TimeChanged;
    }

    public void SetDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void StartClock()
    {
        SetTime();

        clock.Interval = TimeSpan.FromMilliseconds(1000);
        clock.Tick += Clock_Tick;
        clock.Start();

        singleClick.Interval = TimeSpan.FromMilliseconds(SystemInformations.DoubleClickTime);
        singleClick.Tick += SingleClick_Tick;

        ClockVisibility = Visibility.Visible;
    }

    // TODO : Do not remove, call it when disable show clock in user settings
#pragma warning disable S1144, IDE0051 // Unused private types or members should be removed
    private void StopClock()
#pragma warning restore S1144, IDE0051 // Unused private types or members should be removed
    {
        clock.Stop();

        ClockVisibility = Visibility.Collapsed;
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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    private void TimeChanged(object sender, EventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
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

        StringBuilder sb = new();
        foreach (string str in _dateFormat)
        {
            if (!string.IsNullOrWhiteSpace(str) || str == Environment.NewLine)
            {
                try
                {
                    if (str.ToLower() == "<%dayofweek%>")
                        sb.Append(DateTimeFormatInfo.CurrentInfo.GetDayName(now.DayOfWeek));
                    else if (str.StartsWith("<%"))
                        sb.Append(now.ToString(str.Replace("<%", "").Replace("%>", ""), CultureInfo.CurrentCulture));
                    else
                        sb.Append(str);
                }
                catch (Exception) { /* Ignore errors */ }
            }
        }
        ClockText = sb.ToString();

        ClockTip = now.ToString("f", CultureInfo.CurrentCulture);
    }

    private static void OpenDateTimeCpl()
    {
        ShellHelper.StartProcess("timedate.cpl", useShellExecute: true);
    }

    [RelayCommand()]
    private void ShowConfigClock()
    {
        OpenDateTimeCpl();
    }

    [RelayCommand()]
    private void MouseDoubleClick(MouseButtonEventArgs e)
    {
        SingleClickStop();
        OpenDateTimeCpl();

        e.Handled = true;
    }

    [RelayCommand()]
    private void MouseLeftButtonDown()
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_A);
        }
        else
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.LMENU, ManagedShell.Interop.NativeMethods.VK.KEY_D);
        }
    }
}
