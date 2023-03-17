using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

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

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateCopyItem(IntPtr punkItems, IntPtr psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IntPtr pfopsItem);

        private void CopyItemHooked(IntPtr punkItems, IntPtr psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IntPtr pfopsItem)
        {
            MessageBox.Show("Copier");
            //self.CopyItem(punkItems, psiDestinationFolder);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateCopyItems(IntPtr punkItems, IntPtr psiDestinationFolder);

        private void CopyItemsHooked(IntPtr punkItems, IntPtr psiDestinationFolder)
        {
            MessageBox.Show("Copiers");
            //self.CopyItem(punkItems, psiDestinationFolder);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ShFileOpStruct64
        {
            public IntPtr hwnd;
            public uint wFunc;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszProgressTitle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ShFileOpStruct64W
        {
            public IntPtr hwnd;
            public uint wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern int SHFileOperationA(ref ShFileOpStruct64 lpFileOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = false)]
        private delegate int DelegateSHFileOperationA(ref ShFileOpStruct64 lpFileOp);

        private int My_SHFileOperationA(ref ShFileOpStruct64 lpFileOp)
        {
            MessageBox.Show("SHFileOperation");
            //return SHFileOperationA(ref lpFileOp);
            return 0;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern int SHFileOperationW(ref ShFileOpStruct64W lpFileOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate int DelegateSHFileOperationW(ref ShFileOpStruct64W lpFileOp);

        private int My_SHFileOperationW(ref ShFileOpStruct64W lpFileOp)
        {
            MessageBox.Show("SHFileOperation");
            //return SHFileOperationW(ref lpFileOp);
            return 0;
        }
    }
}
