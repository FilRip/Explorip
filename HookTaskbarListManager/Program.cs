using System;
using System.Diagnostics;
using System.Threading;

using EasyHook;

using HookTaskbarList.Interfaces;

namespace HookTaskbarListManager;

internal static class Program
{
    private static string _channelName;

    internal static void Main(string[] args)
    {
        try
        {
            if (args?.Length > 1 && int.TryParse(args[0], out int processId))
            {
                _channelName = args[1];
                TaskbarListServer comm = RemoteHooking.IpcConnectClient<TaskbarListServer>(_channelName);
                comm.Ping();
                comm.ProcessToInjectEvent += Comm_ProcessToInjectEvent;
                while (true)
                {
                    if (Process.GetProcessById(processId) == null)
                        break;
                    comm.Ping();
                    Thread.Sleep(100);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    private static void Comm_ProcessToInjectEvent(object sender, ProcessToInjectEventArgs e)
    {
        try
        {
            RemoteHooking.Inject((int)e.NumProcess,
                InjectionOptions.Default,
                typeof(TaskbarListServer).Assembly.Location,
                typeof(TaskbarListServer).Assembly.Location,
                _channelName);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
