using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

using EasyHook;

namespace Explorip.HookFileOperationsManager
{
    public static class Program
    {
        public static void Main()
        {
            string channelName = null;

            if (Environment.GetCommandLineArgs()[1] == "NI")
            {
                Console.WriteLine("Create channel");
                IpcChannel canal = new("HookFileOperation_" + Process.GetCurrentProcess().Id.ToString());
                ChannelServices.RegisterChannel(canal, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(HookFileOperations.IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
                Console.WriteLine("Channel created");
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            else
            {
                int processId = int.Parse(Environment.GetCommandLineArgs()[1]);

                RemoteHooking.IpcCreateServer<HookFileOperations.ServerInterface>(ref channelName, WellKnownObjectMode.Singleton);
                RemoteHooking.Inject(processId,
                    InjectionOptions.Default,
                    typeof(HookFileOperations.MainHookClass).Assembly.Location,
                    typeof(HookFileOperations.MainHookClass).Assembly.Location,
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
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
