using System;
using System.Runtime.InteropServices;

namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    internal static class ListGuid
    {
        public const string IID_IPersistFile = "0000010b-0000-0000-C000-000000000046";
        public static readonly Guid GUID_IPersistFile = new(IID_IPersistFile);
        
        public const string IID_IPersistIDList = "1079acfc-29bd-11d3-8e0d-00c04f6837d5";
        public static readonly Guid GUID_IPersistIDList = new(IID_IPersistIDList);
    }

    [ComImport, Guid("0000010c-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig()]
        void GetClassID(out Guid pClassID);
    }

    [ComImport, Guid(ListGuid.IID_IPersistFile), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistFile : IPersist
    {
        new void GetClassID(out Guid pClassID);

        [PreserveSig()]
        int IsDirty();

        [PreserveSig()]
        int Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

        [PreserveSig()]
        int Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig()]
        void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig()]
        void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
    }

    [ComImport, Guid(ListGuid.IID_IPersistIDList), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistIDList : IPersist
    {
        new void GetClassID(out Guid pClassID);

        [PreserveSig()]
        int GetIDList(out IntPtr ppidl);

        [PreserveSig()]
        int SetIDList(IntPtr ppidl);
    }
}
