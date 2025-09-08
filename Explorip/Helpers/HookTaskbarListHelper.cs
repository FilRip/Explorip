using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using EasyHook;

using Explorip.TaskBar;

using HookTaskbarList.Interfaces;

using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

namespace Explorip.Helpers;

internal class HookTaskbarListHelper : IDisposable
{
    private static readonly object _lockThreadSafe = new();
    private readonly List<uint> _listProcessInjected = [];

    private TaskbarListServer _server;
    private string _channelName;
    private bool disposedValue;
    private static HookTaskbarListHelper _instance;
    private int _numProcessManaged;

    public static void InstallHook()
    {
        if (_instance == null)
        {
            _instance ??= new HookTaskbarListHelper();
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
        try
        {
            _channelName = null;
            RemoteHooking.IpcCreateServer<TaskbarListServer>(ref _channelName, WellKnownObjectMode.Singleton);
            _server = RemoteHooking.IpcConnectClient<TaskbarListServer>(_channelName);
            _server.AddButtonsEvent += Server_AddButtonsEvent;
            MyTaskbarApp.MyShellManager.TasksService.TaskbarListChanged += TasksService_TaskbarListChanged;

            StartManagerProcess();
            // TODO : Inject in ICustomDestinationList too, later
        }
        catch (Exception ex)
        {
            ShellLogger.Error("Unable to initialize HookTaskbarListHelper: " + ex.ToString());
        }
    }

    private void TasksService_TaskbarListChanged(object sender, WindowEventArgs e)
    {
        if (e.Handle != IntPtr.Zero)
        {
            NativeMethods.GetWindowThreadProcessId(e.Handle, out uint processId);
            if (processId == 0)
                ShellLogger.Debug("WindowEventArgs.ProcessId is 0");
            else
            {
                StartManagerProcess();
                InjectToProcess(processId);
            }
        }
        else
        {
            ShellLogger.Debug("WindowEventArgs.Hndle is null");
        }
    }

    private void StartManagerProcess()
    {
        if (_numProcessManaged == 0 || Process.GetProcessById(_numProcessManaged) == null)
        {
            string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            _numProcessManaged = Process.Start(Path.Combine(path, "HookTaskbarListManager.exe"), Process.GetCurrentProcess().Id.ToString() + " " + _channelName).Id;
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
            if (_listProcessInjected.Contains(processId))
                return;
            _listProcessInjected.Add(processId);
            _server.InjectInProcess(processId);
        }
    }

    #region IDisposable Support

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
                MyTaskbarApp.MyShellManager.TasksService.TaskbarListChanged -= TasksService_TaskbarListChanged;
                _listProcessInjected.Clear();
                try
                {
                    if (_numProcessManaged != 0 && Process.GetProcessById(_numProcessManaged) != null)
                    {
                        Process.GetProcessById(_numProcessManaged).Kill();
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unable to kill HookTaskbarListManager process");
                }
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc/>/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
