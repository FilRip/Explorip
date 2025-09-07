using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using EasyHook;

using HookTaskbarList.Interfaces;
using HookTaskbarList.TaskbarList.Interfaces;

namespace HookTaskbarList;

public class MainHookClass : IEntryPoint
{
    private readonly TaskbarListServer? _server = null;
    private LocalHook? addButtons = null;
    private int _currentProcessId;

#pragma warning disable IDE0060, IDE0079 // Supprimer le paramètre inutilisé
    public MainHookClass(RemoteHooking.IContext context, string channelName)
    {
        if (!string.IsNullOrWhiteSpace(channelName))
        {
            _server = RemoteHooking.IpcConnectClient<TaskbarListServer>(channelName);
            _server.Ping();
        }
    }
#pragma warning restore IDE0060, IDE0079 // Supprimer le paramètre inutilisé

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
    public void Run(RemoteHooking.IContext context, string channelName)
    {
        try
        {
            _currentProcessId = RemoteHooking.GetCurrentProcessId();

            COMClassInfo tblCom = new(typeof(TaskbarList.ClSidITaskbarList), typeof(ITaskbarList), nameof(ITaskbarList.ThumbBarAddButtons));
            tblCom.Query();
            addButtons = LocalHook.Create(tblCom.MethodPointers[0], new DelegateAddButtons(AddButtonsHooked), this);
            addButtons.ThreadACL.SetExclusiveACL([0]);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        if (_server != null)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    _server.Ping();
                }
            }
            catch (Exception) { /* Ignore errors */ }
            Uninstall();
        }
    }

    public void Uninstall()
    {
        addButtons?.Dispose();
        LocalHook.Release();
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
    private delegate void DelegateAddButtons(ITaskbarList self, IntPtr hWndWindow, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] pButtons);
    private void AddButtonsHooked(ITaskbarList self, IntPtr hWndWindow, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] pButtons)
    {
        _server?.AddButtons(_currentProcessId, hWndWindow, cButtons, pButtons);
    }
}
