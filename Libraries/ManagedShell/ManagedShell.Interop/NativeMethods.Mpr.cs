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

    [DllImport(Mpr_DllName)]
    internal static extern int WNetAddConnection2(NetreSource netResource, string password, string username, uint flags);
}
