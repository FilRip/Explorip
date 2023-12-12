﻿using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;

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
    }

    public static class IpcServerManager
    {
        internal const string IpcServerName = "ExploripIpcServer";
        internal const string IpcSubChannelName = "RemoteServer";

        public static void InitChannel(IServerIpc serverInstance)
        {
            try
            {
                IpcChannel channel = new(IpcServerName);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcServerGateway), IpcSubChannelName, WellKnownObjectMode.Singleton);
                IpcServerGateway ipcServer = GetChannel();
                ipcServer.SetInstance(serverInstance);
            }
            catch { /* Ignore errors */ }
        }

        private static IpcServerGateway GetChannel()
        {
            IpcServerGateway serverInstance = (IpcServerGateway)Activator.GetObject(typeof(IpcServerGateway), $"ipc://{IpcServerName}/{IpcSubChannelName}");
            return serverInstance;
        }

        public static void SendNewWindow(string[] args)
        {
            IpcServerGateway channel = GetChannel();
            channel.ReceivedNewWindow(args);
        }

        public static void CreateFolder(string path, string name)
        {
            IpcServerGateway channel = GetChannel();
            channel.CreateFolder(path, name);
        }
    }
}