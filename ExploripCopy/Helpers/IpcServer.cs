using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;

using Explorip.HookFileOperations.Ipc;

namespace ExploripCopy.Helpers
{
    internal static class IpcServer
    {
        private static IpcChannel _channel;

        internal static void CreateIpcServer()
        {
            Console.WriteLine("Create channel");
            _channel = new("ExploripCopy");
            ChannelServices.RegisterChannel(_channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
            Console.WriteLine("Channel created");
            IpcNewInstance ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://ExploripCopy/HookManagerRemoteServer");
            ni.SetMainProcess(new Models.InteractionMainProcess());
        }

        internal static void ShutdownIpcServer()
        {
            ChannelServices.UnregisterChannel(_channel);
        }
    }
}
