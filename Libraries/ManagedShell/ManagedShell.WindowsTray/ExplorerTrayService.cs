﻿using System;
using System.Runtime.InteropServices;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using Microsoft.Win32;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

public class ExplorerTrayService
{
    private SystrayDelegate trayDelegate;

    public ExplorerTrayService()
    {
    }

    internal void SetSystrayCallback(SystrayDelegate theDelegate)
    {
        trayDelegate = theDelegate;
    }

    internal void Run()
    {
        if (!EnvironmentHelper.IsAppRunningAsShell && trayDelegate != null)
        {
            bool autoTrayEnabled = GetAutoTrayEnabled();

            if (autoTrayEnabled)
            {
                // we can't get tray icons that are in the hidden area, so disable that temporarily if enabled
                try
                {
                    TrayNotify trayNotify = new();

                    SetAutoTrayEnabled(trayNotify, false);
                    GetTrayItems();
                    SetAutoTrayEnabled(trayNotify, true);

                    Marshal.ReleaseComObject(trayNotify);
                }
                catch (Exception e)
                {
                    ShellLogger.Debug($"ExplorerTrayService: Unable to get items using ITrayNotify: {e.Message}");
                }
            }
            else
            {
                try
                {
                    GetTrayItems();
                }
                catch (Exception e)
                {
                    ShellLogger.Debug($"ExplorerTrayService: Unable to get items: {e.Message}");
                }
            }
        }
    }

    private void GetTrayItems()
    {
        IntPtr toolbarHwnd = FindExplorerTrayToolbarHwnd();

        if (toolbarHwnd == IntPtr.Zero)
        {
            return;
        }

        int count = GetNumTrayIcons(toolbarHwnd);

        if (count < 1)
        {
            return;
        }

        GetWindowThreadProcessId(toolbarHwnd, out uint processId);
        IntPtr hProcess = OpenProcess(ProcessAccess.All, false, (int)processId);
        IntPtr hBuffer = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)Marshal.SizeOf(new TbButton()), AllocationType.Commit,
            MemoryProtection.ReadWrite);

        for (int i = 0; i < count; i++)
        {
            TrayItem trayItem = GetTrayItem(i, hBuffer, hProcess, toolbarHwnd);

            if (trayItem.hWnd == IntPtr.Zero || !IsWindow(trayItem.hWnd))
            {
                ShellLogger.Debug($"ExplorerTrayService: Ignored notify icon {trayItem.szIconText} due to invalid handle");
                continue;
            }

            SafeNotifyIconData nid = GetTrayItemIconData(trayItem);

            if (trayDelegate != null)
            {
                if (!trayDelegate((uint)NIM.NIM_ADD, nid))
                {
                    ShellLogger.Debug("ExplorerTrayService: Ignored notify icon message");
                }
            }
            else
            {
                ShellLogger.Debug("ExplorerTrayService: trayDelegate is null");
            }
        }

        VirtualFreeEx(hProcess, hBuffer, 0, AllocationType.Release);

        CloseHandle((int)hProcess);
    }

    private static IntPtr FindExplorerTrayToolbarHwnd()
    {
        IntPtr hwnd = FindWindow(WindowHelper.TrayWndClass, "");

        if (hwnd != IntPtr.Zero)
        {
            hwnd = FindWindowEx(hwnd, IntPtr.Zero, "TrayNotifyWnd", "");

            if (hwnd != IntPtr.Zero)
            {
                hwnd = FindWindowEx(hwnd, IntPtr.Zero, "SysPager", "");

                if (hwnd != IntPtr.Zero)
                {
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", IntPtr.Zero);

                    return hwnd;
                }
            }
        }

        return IntPtr.Zero;
    }

    private static int GetNumTrayIcons(IntPtr toolbarHwnd)
    {
        return (int)SendMessage(toolbarHwnd, (int)TB.BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);
    }

    private static TrayItem GetTrayItem(int i, IntPtr hBuffer, IntPtr hProcess, IntPtr toolbarHwnd)
    {
        TbButton tbButton = new();
        TrayItem trayItem = new();
        IntPtr hTBButton = Marshal.AllocHGlobal(Marshal.SizeOf(tbButton));
        IntPtr hTrayItem = Marshal.AllocHGlobal(Marshal.SizeOf(trayItem));

        _ = SendMessage(toolbarHwnd, (int)TB.GETBUTTON, (IntPtr)i, hBuffer);
        if (ReadProcessMemory(hProcess, hBuffer, hTBButton, Marshal.SizeOf(tbButton), out _))
        {
            tbButton = (TbButton)Marshal.PtrToStructure(hTBButton, typeof(TbButton));

            if (tbButton.dwData != UIntPtr.Zero &&
                ReadProcessMemory(hProcess, tbButton.dwData, hTrayItem, Marshal.SizeOf(trayItem), out _))
            {
                trayItem = (TrayItem)Marshal.PtrToStructure(hTrayItem, typeof(TrayItem));

                if ((tbButton.FsState & TBSTATE_HIDDEN) != 0)
                {
                    trayItem.dwState = 1;
                }
                else
                {
                    trayItem.dwState = 0;
                }

                ShellLogger.Debug($"ExplorerTrayService: Got tray item: {trayItem.szIconText}");
            }
        }

        return trayItem;
    }

    private SafeNotifyIconData GetTrayItemIconData(TrayItem trayItem)
    {
        SafeNotifyIconData nid = new()
        {
            hWnd = trayItem.hWnd,
            uID = trayItem.uID,
            uCallbackMessage = trayItem.uCallbackMessage,
            szTip = trayItem.szIconText,
            hIcon = trayItem.hIcon,
            uVersion = trayItem.uVersion,
            guidItem = trayItem.guidItem,
            dwState = (int)trayItem.dwState,
            uFlags = NIF.GUID | NIF.MESSAGE | NIF.TIP | NIF.STATE
        };

        if (nid.hIcon != IntPtr.Zero)
        {
            nid.uFlags |= NIF.ICON;
        }
        else
        {
            ShellLogger.Warning($"ExplorerTrayService: Unable to use {trayItem.szIconText} icon handle for NOTIFYICONDATA struct");
        }

        return nid;
    }

    private static bool GetAutoTrayEnabled()
    {
        int enableAutoTray = 1;

        try
        {
            RegistryKey explorerKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer", false);

            if (explorerKey != null)
            {
                object enableAutoTrayValue = explorerKey.GetValue("EnableAutoTray");

                if (enableAutoTrayValue != null)
                {
                    enableAutoTray = Convert.ToInt32(enableAutoTrayValue);
                }
            }
        }
        catch (Exception e)
        {
            ShellLogger.Debug($"ExplorerTrayService: Unable to get EnableAutoTray setting: {e.Message}");
        }

        return enableAutoTray == 1;
    }

    private static void SetAutoTrayEnabled(TrayNotify trayNotify, bool enabled)
    {
        try
        {
            if (EnvironmentHelper.IsWindows8OrBetter)
            {
                ITrayNotify trayNotifyInstance = (ITrayNotify)trayNotify;
                trayNotifyInstance.EnableAutoTray(enabled);
            }
            else
            {
                ITrayNotifyLegacy trayNotifyInstance = (ITrayNotifyLegacy)trayNotify;
                trayNotifyInstance.EnableAutoTray(enabled);
            }
        }
        catch (Exception e)
        {
            ShellLogger.Debug($"ExplorerTrayService: Unable to set EnableAutoTray setting: {e.Message}");
        }
    }

    private const byte TBSTATE_HIDDEN = 8;

    private enum TB : uint
    {
        GETBUTTON = WM.USER + 23,
        BUTTONCOUNT = WM.USER + 24
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TrayItem
    {
        public IntPtr hWnd;
        public uint uID;
        public uint uCallbackMessage;
        public uint dwState;
        public uint uVersion;
        public IntPtr hIcon;
        public IntPtr uIconDemoteTimerID;
        public uint dwUserPref;
        public uint dwLastSoundTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szIconText;
        public uint uNumSeconds;
        public Guid guidItem;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TbButton
    {
        public int iBitmap;
        public int idCommand;
#pragma warning disable IDE0044
        [StructLayout(LayoutKind.Explicit)]
        private struct TbButtonU
        {
            [FieldOffset(0)] public byte fsState;
            [FieldOffset(1)] public byte fsStyle;
            [FieldOffset(0)] private IntPtr bReserved;
        }
#pragma warning restore IDE0044
        private TbButtonU union;
        public byte FsState { get { return union.fsState; } set { union.fsState = value; } }
        public byte FsStyle { get { return union.fsStyle; } set { union.fsStyle = value; } }
        public UIntPtr dwData;
        public IntPtr iString;
    }
}
