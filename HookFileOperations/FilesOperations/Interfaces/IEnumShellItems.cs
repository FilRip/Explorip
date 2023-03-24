using System;
using System.Runtime.InteropServices;

namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    [ComImport]
    [Guid("70629033-e363-4a28-a567-0db78006e6d7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumShellItems
    {
        /// <summary>
        /// Gets an array of one or more IShellItem interfaces from the enumeration.
        /// </summary>
        /// <param name="celt"></param>
        /// <param name="rgelt"></param>
        /// <param name="pceltFetched"></param>
        /// <returns></returns>
        int Next(uint celt, out IShellItem rgelt, out uint pceltFetched);

        /// <summary>
        /// Skips a given number of IShellItem interfaces in the enumeration.
        /// Used when retrieving interfaces.
        /// </summary>
        /// <param name="celt"></param>
        /// <returns></returns>
        int Skip(uint celt);

        /// <summary>
        /// Resets the internal count of retrieved IShellItem interfaces in the enumeration.
        /// </summary>
        /// <returns></returns>
        int Reset();

        /// <summary>
        /// Gets a copy of the current enumeration.
        /// </summary>
        /// <param name="ppenum"></param>
        /// <returns></returns>
        int Clone(out IEnumShellItems ppenum);
    }
}
