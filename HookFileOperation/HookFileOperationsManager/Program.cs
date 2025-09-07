using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

using EasyHook;

using Explorip.HookFileOperations;
using Explorip.HookFileOperations.Ipc;

namespace Explorip.HookFileOperationsManager;

public static class Program
{
    public static void Main()
    {
        string channelName = null;

        if (Environment.GetCommandLineArgs().Length > 1)
        {
            if (Environment.GetCommandLineArgs()[1] == "NI")
            {
                IpcChannel canal = new("HookFileOperation_" + Process.GetCurrentProcess().Id.ToString());
                ChannelServices.RegisterChannel(canal, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            else if (int.TryParse(Environment.GetCommandLineArgs()[1], out int processId))
            {
                RemoteHooking.IpcCreateServer<ServerInterface>(ref channelName, WellKnownObjectMode.Singleton);
                RemoteHooking.Inject(processId,
                    InjectionOptions.Default,
                    typeof(MainHookClass).Assembly.Location,
                    typeof(MainHookClass).Assembly.Location,
                    channelName);

                try
                {
                    while (true)
                    {
                        if (Process.GetProcessById(processId) == null)
                            break;
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
