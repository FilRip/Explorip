using System.Diagnostics;

using EasyHook;

namespace Explorip.Helpers
{
    internal class HookCopyOperations
    {
        private static HookCopyOperations _instance;
        private HookFileOperations.MainHookClass _classeInjectLocal;

        public static HookCopyOperations GetInstance()
        {
            if (_instance == null)
                _instance = new HookCopyOperations();
            return _instance;
        }

        internal void InstallHook()
        {
            string channelName = null;
            RemoteHooking.IpcCreateServer<HookFileOperations.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            _classeInjectLocal = new HookFileOperations.MainHookClass(null, null);
            _classeInjectLocal.Run(null, null);

            int pidExplorer;
            pidExplorer = Process.GetProcessesByName("explorer")[0].Id;
            RemoteHooking.Inject(pidExplorer,
                InjectionOptions.Default,
                typeof(HookFileOperations.MainHookClass).Assembly.Location,
                typeof(HookFileOperations.MainHookClass).Assembly.Location,
                channelName);
        }

        internal void UninstallHook()
        {
            _classeInjectLocal.Uninstall();
        }
    }
}
