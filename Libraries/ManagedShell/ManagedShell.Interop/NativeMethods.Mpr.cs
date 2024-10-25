using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    private const string Mpr_DllName = "mpr.dll";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct UniversalNameInfo
    {
        /// <summary>
        /// Pointer to the null-terminated UNC name string that identifies a
        /// network resource.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpUniversalName;
    }

    public enum InfoLevel
    {
        /// <summary>
        /// The function stores a <see cref="UniversalNameInfo"/> structure in the
        /// buffer.
        /// </summary>
        UniversalName = 1,

        /// <summary>
        /// The function stores a <c>REMOTE_NAME_INFO</c> structure in the buffer.
        /// </summary>
        /// <remarks>
        /// Using this level will throw an <see cref="NotSupportedException"/>.
        /// </remarks>
        RemoteName = 2
    }

    [DllImport(Mpr_DllName, CharSet = CharSet.Auto)]
    internal static extern int WNetGetUniversalName(
        string lpLocalPath,
        InfoLevel dwInfoLevel,
        ref UniversalNameInfo lpBuffer,
        ref int lpBufferSize);

    [DllImport(Mpr_DllName, CharSet = CharSet.Auto)]
    internal static extern int WNetGetUniversalName(
        string lpLocalPath,
        InfoLevel dwInfoLevel,
        IntPtr lpBuffer,
        ref int lpBufferSize);

    [DllImport(Mpr_DllName)]
    internal static extern uint WNetGetConnection(string lpLocalName, StringBuilder lpRemoteName, ref int lpnLength);

    public enum ResourceScope
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    }

    [Flags()]
    public enum ResourceType
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }

    [Flags()]
    public enum ResourceUsage : uint
    {
        Connectable = 0x1,
        Container = 0x2,
        NoLocalDevice = 0x4,
        Sibling = 0x8,
        Attached = 0x10,
        All = Connectable | Container | Attached,
        Reserved = 0x80000000u
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NetResource
    {
        public ResourceScope dwScope;
        public ResourceType dwType;
        public ResourceDisplaytype dwDisplayType;
        public ResourceUsage dwUsage;
        [MarshalAs(UnmanagedType.LPTStr)] public string LocalName;
        [MarshalAs(UnmanagedType.LPTStr)] public string RemoteName;
        [MarshalAs(UnmanagedType.LPTStr)] public string Comment;
        [MarshalAs(UnmanagedType.LPTStr)] public string Provider;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NetreSource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum WNetOpenEnumError
    {
        NERR_Success = 0,
        ERROR_ACCESS_DENIED = 5,
        ERROR_NOT_ENOUGH_MEMORY = 8,
        ERROR_BAD_NETPATH = 53,
        ERROR_NETWORK_BUSY = 54,
        ERROR_INVALID_PARAMETER = 87,
        ERROR_INVALID_LEVEL = 124,
        ERROR_MORE_DATA = 234,
        ERROR_EXTENDED_ERROR = 1208,
        ERROR_NO_NETWORK = 1222,
        ERROR_INVALID_HANDLE_STATE = 1609,
        ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
    }

    [DllImport(Mpr_DllName)]
    internal static extern int WNetAddConnection2(NetreSource netResource, string password, string username, uint flags);

    [DllImport(Mpr_DllName, CharSet = CharSet.Auto)]
    internal static extern int WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NetResource lpNetResource, ref IntPtr lphEnum);

    [DllImport(Mpr_DllName, CharSet = CharSet.Auto)]
    internal static extern int WNetEnumResource(IntPtr hEnum, ref int lpcCount, IntPtr lpBuffer, ref uint lpBufferSize);

    [DllImport(Mpr_DllName)]
    internal static extern int WNetCloseEnum(IntPtr hEnum);

    [DllImport(Mpr_DllName, CharSet = CharSet.Auto, SetLastError = false)]
    internal static extern int WNetGetLastError(out uint lpError, StringBuilder lpErrorBuf, uint nErrorBufSize, StringBuilder lpNameBuf, uint nNameBufSize);
}
