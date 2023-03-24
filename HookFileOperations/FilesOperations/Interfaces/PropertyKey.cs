using System;
using System.Runtime.InteropServices;

namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    /// <summary>
    /// Defines a unique key for a Shell Property
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PROPERTYKEY
    {
        public Guid fmtid;
        public uint pid;
    }
}
