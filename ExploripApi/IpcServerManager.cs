using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

namespace ExploripApi
{
    public class IpcServerGateway : MarshalByRefObject
    {
        private IServerIpc _serverInstance;

        public void SetInstance(IServerIpc serverInstance)
        {
            _serverInstance = serverInstance;
        }

        public void CreateFolder(string folderPath, string folderName)
        {
            _serverInstance?.CreateFolder(folderPath, folderName);
        }

        public void ReceivedNewWindow(string[] args)
        {
            _serverInstance?.ReceivedNewWindow(args);
        }

        public void CreateShortcut(string folderPath, string name)
        {
            _serverInstance?.CreateShortcut(folderPath, name);
        }

        public void Ping()
        {
            _serverInstance?.Ping();
        }
    }

    public static class IpcServerManager
    {
        internal const string IpcServerName = "ExploripIpcServer";
        internal const string IpcSubChannelName = "RemoteServer";
        private static Thread _keepAlive;

        public static void InitChannel(IServerIpc serverInstance)
        {
            try
            {
                IpcChannel channel = new(IpcServerName);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcServerGateway), IpcSubChannelName, WellKnownObjectMode.Singleton);
                IpcServerGateway ipcServer = GetChannel();
                ipcServer.SetInstance(serverInstance);
                _keepAlive = new Thread(new ThreadStart(KeepAliveThread))
                {
                    IsBackground = true,
                };
                _keepAlive.Start();
            }
            catch { /* Ignore errors */ }
        }

        public static void Shutdown()
        {
            _keepAlive?.Abort();
        }

        private static void KeepAliveThread()
        {
            while (true)
            {
                try
                {
                    GetChannel().Ping();
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private static IpcServerGateway GetChannel()
        {
            IpcServerGateway serverInstance = (IpcServerGateway)Activator.GetObject(typeof(IpcServerGateway), $"ipc://{IpcServerName}/{IpcSubChannelName}");
            return serverInstance;
        }

        public static void SendNewWindow(string[] args)
        {
            IpcServerGateway channel = GetChannel();
            channel?.ReceivedNewWindow(args);
        }

        public static void CreateFolder(string path, string name)
        {
            IpcServerGateway channel = GetChannel();
            channel?.CreateFolder(path, name);
        }

        public static void CreateShortcut(string path, string name)
        {
            IpcServerGateway channel = GetChannel();
            channel?.CreateShortcut(path, name);
        }
    }
}
