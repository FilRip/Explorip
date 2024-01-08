using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

using Explorip.HookFileOperations.Ipc;

namespace ExploripCopy.Helpers
{
    internal static class IpcServer
    {
        private static IpcChannel _channel;
        private static IpcNewInstance _ni;
        private static Thread _keepAlive;

        internal static void CreateIpcServer()
        {
            _channel = new("ExploripCopy");
            ChannelServices.RegisterChannel(_channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcNewInstance), "HookManagerRemoteServer", WellKnownObjectMode.Singleton);
            _ni = (IpcNewInstance)Activator.GetObject(typeof(IpcNewInstance), $"ipc://ExploripCopy/HookManagerRemoteServer");
            _ni.SetMainProcess(new Models.InteractionMainProcess());
            _keepAlive = new Thread(new ThreadStart(KeepAliveThread))
            {
                IsBackground = true,
            };
            _keepAlive.Start();
        }

        private static void KeepAliveThread()
        {
            while (true)
            {
                try
                {
                    _ni?.IsReady();
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        internal static void ShutdownIpcServer()
        {
            try
            {
                _keepAlive?.Abort();
                _ni.SetMainProcess(null);
                ChannelServices.UnregisterChannel(_channel);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }
}
