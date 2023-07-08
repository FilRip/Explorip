using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows;

using Explorip.Explorer.Windows;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Helpers
{
    internal sealed class IpcServer : MarshalByRefObject
    {
        internal static readonly string IpcServerName = "ExploripIpcServer";
        internal static readonly string IpcSubChannelName = "RemoteServer";

        public void ReceivedNewWindow(string[] args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WpfExplorerBrowser newExplorerWindow = new(false);
                newExplorerWindow.LeftTab.FirstTab.Navigation(ShellObject.FromParsingName(args[0]));
                newExplorerWindow.RightTab.CloseAllTabs();
                newExplorerWindow.RightTab.HideTab();
                newExplorerWindow.Show();
            });
        }
    }

    internal static class IpcServerManager
    {
        internal static void InitChannel()
        {
            try
            {
                IpcChannel channel = new(IpcServer.IpcServerName);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcServer), IpcServer.IpcSubChannelName, WellKnownObjectMode.Singleton);
            }
            catch { /* Ignore errors */ }
        }

        internal static void SendMessage(string[] args)
        {
            IpcChannel channel = new();
            ChannelServices.RegisterChannel(channel, false);
            IpcServer serverInstance = (IpcServer)Activator.GetObject(typeof(IpcServer), $"ipc://{IpcServer.IpcServerName}/{IpcServer.IpcSubChannelName}");
            serverInstance.ReceivedNewWindow(args);
        }
    }
}
