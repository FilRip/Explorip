using System;
using System.Windows;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

namespace Explorip.TaskBar.Models;

public static class HookRefreshLanguageLayout
{
    private static readonly object _lockThreadSafe = new();
    private static IntPtr _keyboardHookPtr = IntPtr.Zero;

    public static void Hook()
    {
        lock (_lockThreadSafe)
        {
            if (_keyboardHookPtr == IntPtr.Zero)
                _keyboardHookPtr = NativeMethods.SetWindowsHookEx(NativeMethods.HookType.WH_KEYBOARD_LL, MyKeyboardHook, IntPtr.Zero, 0);
        }
    }

    private static int MyKeyboardHook(int code, int wParam, ref NativeMethods.KeyboardHookStruct lParam)
    {
        if (code >= 0 && wParam == (int)NativeMethods.WM.KEYUP &&
            (lParam.VkCode == (int)NativeMethods.VK.LWIN || lParam.VkCode == (int)NativeMethods.VK.RWIN) && Application.Current is MyTaskbarApp myApp)
        {
            foreach (Taskbar tb in myApp.ListAllTaskbar())
                if (ConfigManager.GetTaskbarConfig(tb.NumScreen).ShowKeyboardLayout)
                    tb.LanguageLayoutButton.MyDataContext.RefreshCurrentLayout();
        }
        return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
    }

    public static void UnHook()
    {
        lock (_lockThreadSafe)
        {
            if (_keyboardHookPtr != IntPtr.Zero)
                NativeMethods.UnhookWindowsHookEx(_keyboardHookPtr);
        }
    }
}
