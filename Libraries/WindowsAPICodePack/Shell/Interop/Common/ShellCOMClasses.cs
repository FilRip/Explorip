using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Shell.Interop.Common;

[ComImport(),
Guid(ShellIidGuid.IShellLibrary),
CoClass(typeof(ShellLibraryCoClass))]
internal interface INativeShellLibrary : IShellLibrary
{
}

[ComImport(),
ClassInterface(ClassInterfaceType.None),
TypeLibType(TypeLibTypeFlags.FCanCreate),
Guid(ShellClSidGuid.ShellLibrary)]
internal class ShellLibraryCoClass
{
}
