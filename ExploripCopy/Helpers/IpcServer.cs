using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;

using Explorip.HookFileOperations;
using Explorip.HookFileOperations.Ipc;

namespace ExploripCopy.Helpers
{
    internal static class IpcServer
    {
        internal static void CreateIpcServer()
        {
            Console.WriteLine("Create channel");
            IpcChannel canal = new("ExploripCopy");
            ChannelServices.RegisterChannel(canal, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
            Console.WriteLine("Channel created");
            IpcNewInstance ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://ExploripCopy/HookManagerRemoteServer");
            ni.SetMainProcess(new Models.InteractionMainProcess());
        }
    }
}
