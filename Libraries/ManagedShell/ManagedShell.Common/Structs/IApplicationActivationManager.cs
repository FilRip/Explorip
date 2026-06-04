using System;
using System.Runtime.InteropServices;

using Explorip.HookFileOperations.FilesOperations.Interfaces;

namespace ManagedShell.Common.Structs;

[ComImport()]
[Guid("2e941141-7f97-4756-ba1d-9decde894a3d")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IApplicationActivationManager
{
    int ActivateApplication([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
                            [MarshalAs(UnmanagedType.LPWStr)] string arguments,
                            int options,
                            out uint processId);

    int ActivateForFile([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
                        IShellItemArray itemArray,
                        [MarshalAs(UnmanagedType.LPWStr)] string verb,
                        out uint processId);

    int ActivateForProtocol([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
                            IShellItemArray itemArray,
                            out uint processId);
}

[ComImport()]
[Guid("b4cb50e9-4533-4f5d-ae63-a0e3f9f5e43b")]
public class ApplicationActivationManager
{
}
