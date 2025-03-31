using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class SearchBarViewModel : ObservableObject
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
        if (height > 0)
            Height = height;
    }

    [RelayCommand()]
    private void KeyDown(KeyEventArgs e)
    {
        if ((e.Key == Key.Enter || e.Key == Key.Return) && !string.IsNullOrWhiteSpace(SearchText))
            ShowSearch();
    }

    private void ShowSearch()
    {
        IntPtr windowPtr = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.SEARCH);
        if (windowPtr != IntPtr.Zero)
        {
            NativeMethods.ShowWindow(windowPtr, NativeMethods.WindowShowStyle.ShowNormal);
            ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_S);
            Thread.Sleep(200);
            List<NativeMethods.Input> listKeys = [];
            NativeMethods.Input i;
            foreach (char c in SearchText)
            {
                i = new()
                {
                    type = NativeMethods.INPUT_KEYBOARD,
                    mkhi = new NativeMethods.MouseKeybdHardwareInputUnion() { ki = new NativeMethods.KeyBDInput() { wVk = c, } }
                };
                listKeys.Add(i);
                i = new()
                {
                    type = NativeMethods.INPUT_KEYBOARD,
                    mkhi = new NativeMethods.MouseKeybdHardwareInputUnion() { ki = new NativeMethods.KeyBDInput() { wVk = c, dwFlags = NativeMethods.KEYEVENTF_KEYUP } }
                };
                listKeys.Add(i);
            }
            uint ret = NativeMethods.SendInput((uint)listKeys.Count, [.. listKeys], Marshal.SizeOf(typeof(NativeMethods.Input)));
            System.Diagnostics.Debug.WriteLine("Key pressed in search : " + ret);
        }
    }
}
