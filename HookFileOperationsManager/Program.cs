using System;
using System.Diagnostics;
using System.Threading;

using EasyHook;

namespace Explorip.HookFileOperationsManager
{
    public static class Program
    {
        public static void Main()
        {
            string channelName = null;
            int processId = int.Parse(Environment.GetCommandLineArgs()[1]);

            RemoteHooking.IpcCreateServer<HookFileOperations.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
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
