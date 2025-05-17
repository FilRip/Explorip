using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

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
        ShellHelper.ShellKeyCombo(VK.LWIN, VK.KEY_S);
        Stopwatch stopwatch = Stopwatch.StartNew();
        IntPtr ptrForegroundWindow = IntPtr.Zero;
        while (stopwatch.ElapsedMilliseconds < 2000)
        {
            Thread.Sleep(10);
            ptrForegroundWindow = GetForegroundWindow();
            StringBuilder sb = new(256);
            GetClassName(ptrForegroundWindow, sb, 256);
            if (sb.ToString() == "Windows.UI.Core.CoreWindow")
                break;
        }
        stopwatch.Stop();
        if (stopwatch.ElapsedMilliseconds >= 2000)
            return;
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        Taskbar currentTaskbar = ((MyTaskbarApp)Application.Current).ListAllTaskbar().FirstOrDefault(t => t.ScreenName == screen.DeviceName.TrimStart('.', '\\'));
        if (currentTaskbar != null)
        {
            GetWindowRect(ptrForegroundWindow, out ManagedShell.Interop.NativeMethods.Rect rect);
            SetWindowPos(ptrForegroundWindow, IntPtr.Zero, (int)currentTaskbar.Left, (int)(currentTaskbar.Top * screen.ScaleFactor) - rect.Height, rect.Width, rect.Height, NativeMethods.SWP.SWP_SHOWWINDOW);
        }
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

    [RelayCommand()]
    private void GotFocus()
    {
        if (SearchText == Constants.Localization.SEARCH)
            SearchText = "";
    }
}
