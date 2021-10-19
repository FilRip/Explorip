﻿using System;
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
            /// The function stores a <see cref="UNIVERSAL_NAME_INFO"/> structure in the
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
        public static extern int WNetGetUniversalName(
            string lpLocalPath,
            InfoLevel dwInfoLevel,
            ref UNIVERSAL_NAME_INFO lpBuffer,
            ref int lpBufferSize);

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetGetUniversalName(
            string lpLocalPath,
            InfoLevel dwInfoLevel,
            IntPtr lpBuffer,
            ref int lpBufferSize);

        [DllImport("mpr.dll")]
        public static extern uint WNetGetConnection(string lpLocalName, StringBuilder lpRemoteName, ref int lpnLength);

        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2(NETRESOURCE netResource, string password, string username, uint flags);
    }
}
