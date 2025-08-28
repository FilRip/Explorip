using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using HookTaskbarList.Interfaces;

namespace Explorip.Helpers;

internal static class HookTaskbarList
{
    internal static void RegisterIpcChannel()
    {
        IpcChannel canal = new("HookTaskbarList");
        ChannelServices.RegisterChannel(canal, false);
        RemotingConfiguration.RegisterWellKnownServiceType(typeof(TaskbarListServer), "HookTaskbarList", WellKnownObjectMode.Singleton);
    }
}
