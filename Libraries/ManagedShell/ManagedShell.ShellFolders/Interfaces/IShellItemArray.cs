using System;
using System.Runtime.InteropServices;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport(), Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItemArray
{
    // Not supported: IBindCtx
    void BindToHandler([MarshalAs(UnmanagedType.Interface)] IntPtr pbc, ref Guid rbhid,
                  ref Guid riid, out IntPtr ppvOut);

    void GetPropertyStore(int Flags, ref Guid riid, out IntPtr ppv);

    void GetPropertyDescriptionList(ref NativeMethods.PropertyKey keyType, ref Guid riid, out IntPtr ppv);

    void GetAttributes(SiAttrib dwAttribFlags, uint sfgaoMask, out uint psfgaoAttribs);

    void GetCount(out uint pdwNumItems);

    void GetItemAt(uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
    void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
}
