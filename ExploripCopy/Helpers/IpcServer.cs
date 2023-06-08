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
        private static IpcNewInstance _ni;

        internal static void CreateIpcServer()
        {
            Console.WriteLine("Create channel");
            _channel = new("ExploripCopy");
            ChannelServices.RegisterChannel(_channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
            Console.WriteLine("Channel created");
            _ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://ExploripCopy/HookManagerRemoteServer");
            _ni.SetMainProcess(new Models.InteractionMainProcess());
        }

        internal static void ShutdownIpcServer()
        {
            try
            {
                _ni.SetMainProcess(null);
                ChannelServices.UnregisterChannel(_channel);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }
}
