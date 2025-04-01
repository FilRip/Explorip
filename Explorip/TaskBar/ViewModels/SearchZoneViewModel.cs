using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.TaskBar.ViewModels;

public partial class SearchZoneViewModel : ObservableObject
{
    [ObservableProperty()]
    private string _searchText;
    [ObservableProperty()]
    private double _height, _width;

    private Taskbar _parentTaskbar;

    public void SetTaskbar(Taskbar taskbar)
    {
        _parentTaskbar = taskbar;
        Width = ConfigManager.GetTaskbarConfig(_parentTaskbar.ScreenName).SearchWidth;
        double height = ConfigManager.GetTaskbarConfig(_parentTaskbar.ScreenName).SearchHeight;
        Height = height > 0 ? height : ConfigManager.GetTaskbarConfig(_parentTaskbar.ScreenName).TaskButtonSize;
        SearchText = Constants.Localization.SEARCH;
    }

    [RelayCommand()]
    private void KeyDown(KeyEventArgs e)
    {
        if ((e.Key == Key.Enter || e.Key == Key.Return) && !string.IsNullOrWhiteSpace(SearchText))
            ShowSearch();
    }

    private void ShowSearch()
    {
        IntPtr windowPtr = FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.SEARCH);
        if (windowPtr != IntPtr.Zero)
        {
            ShowWindow(windowPtr, WindowShowStyle.ShowNormal);
            ShellHelper.ShellKeyCombo(VK.LWIN, VK.KEY_S);
            Stopwatch stopwatch = Stopwatch.StartNew();
            IntPtr ptrForegroundWindow;
            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                Thread.Sleep(10);
                ptrForegroundWindow = GetForegroundWindow();
                if (ptrForegroundWindow == windowPtr)
                    break;
            }
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds >= 2000)
                return;
            Thread.Sleep(200);
            List<Input> listKeys = [];
            Input i;
            foreach (char c in SearchText)
            {
                i = new()
                {
                    type = TypeInput.Keyboard,
                    mkhi = new MouseKeybdHardwareInputUnion() { ki = new KeyBDInput() { wScan = c, dwFlags = KeyEventF.KeyDown | KeyEventF.Unicode } }
                };
                listKeys.Add(i);
                i = new()
                {
                    type = TypeInput.Keyboard,
                    mkhi = new MouseKeybdHardwareInputUnion() { ki = new KeyBDInput() { wScan = c, dwFlags = KeyEventF.KeyUp | KeyEventF.Unicode } }
                };
                listKeys.Add(i);
            }
            SendInput((uint)listKeys.Count, [.. listKeys], Marshal.SizeOf(typeof(Input)));
        }
    }

    [RelayCommand()]
    private void GotFocus()
    {
        if (SearchText == Constants.Localization.SEARCH)
            SearchText = "";
    }
}
