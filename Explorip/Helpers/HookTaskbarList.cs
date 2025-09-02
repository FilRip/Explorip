using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;

using EasyHook;

using Explorip.TaskBar;

using HookTaskbarList.Interfaces;

using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

namespace Explorip.Helpers;

internal class HookTaskbarList : IDisposable
{
    private string _channelName;
    private TaskbarListServer _server;
    private static readonly object _lockThreadSafe = new();
    private List<uint> _alreadyInjectedProcess;
    private bool disposedValue;
    private static HookTaskbarList _instance;

    public static void InstallHook()
    {
        if (_instance == null)
        {
            _instance ??= new HookTaskbarList();
            _instance.Init();
        }
    }

    public static void UninstallHook()
    {
        if (_instance != null)
        {
            _instance.Dispose();
            _instance = null;
        }
    }

    internal void Init()
    {
        _alreadyInjectedProcess = [];
        _server = new TaskbarListServer();
        _channelName = null;
        RemoteHooking.IpcCreateServer<TaskbarListServer>(ref _channelName, WellKnownObjectMode.Singleton);
        _server.AddButtonsEvent += Server_AddButtonsEvent;
        MyTaskbarApp.MyShellManager.TasksService.TaskbarListChanged += TasksService_TaskbarListChanged;
    }

    private void TasksService_TaskbarListChanged(object sender, WindowEventArgs e)
    {
        if (e.Handle != IntPtr.Zero)
        {
            NativeMethods.GetWindowThreadProcessId(e.Handle, out uint processId);
            if (processId == 0)
                ShellLogger.Debug("WindowEventArgs.ProcessId is 0");
            else
                InjectToProcess(processId);
        }
        else
        {
            ShellLogger.Debug("WindowEventArgs.Hndle is null");
        }
    }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    private void Server_AddButtonsEvent(object sender, AddButtonsEventArgs e)
    {
        ShellLogger.Debug($"Process {e.ProcessId} has sent {e.NbButtons} Thumbnail Buttons for window {e.Handle}");
        ApplicationWindow appWin = MyTaskbarApp.MyShellManager.TasksService.Windows.SingleOrDefault(w => w.ListWindows?.Count > 0 && w.ListWindows.Contains(e.Handle));
        appWin?.SetThumbButtons(e.Buttons);
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    internal void InjectToProcess(uint processId)
    {
        lock (_lockThreadSafe)
        {
            if (_alreadyInjectedProcess.Contains(processId))
                return;

            ShellLogger.Debug($"Injecting HookTaskbarList into process {processId}");
            try
            {
                RemoteHooking.Inject((int)processId,
                    InjectionOptions.Default,
                    typeof(TaskbarListServer).Assembly.Location,
                    typeof(TaskbarListServer).Assembly.Location,
                    _channelName);

                _alreadyInjectedProcess.Add(processId);
            }
            catch (Exception ex)
            {
                ShellLogger.Error($"Failed to inject HookTaskbarList into process {processId}: {ex.Message}");
            }
        }
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
                _server.AddButtonsEvent -= Server_AddButtonsEvent;
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
