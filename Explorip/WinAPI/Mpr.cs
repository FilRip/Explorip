using System;
using System.Runtime.InteropServices;
using System.Text;

using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class Mpr
    {
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

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        internal static extern int WNetGetUniversalName(
            string lpLocalPath,
            InfoLevel dwInfoLevel,
            ref UniversalNameInfo lpBuffer,
            ref int lpBufferSize);

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        internal static extern int WNetGetUniversalName(
            string lpLocalPath,
            InfoLevel dwInfoLevel,
            IntPtr lpBuffer,
            ref int lpBufferSize);

        [DllImport("mpr.dll")]
        internal static extern uint WNetGetConnection(string lpLocalName, StringBuilder lpRemoteName, ref int lpnLength);

        [DllImport("mpr.dll")]
        internal static extern int WNetAddConnection2(NetreSource netResource, string password, string username, uint flags);
    }
}
