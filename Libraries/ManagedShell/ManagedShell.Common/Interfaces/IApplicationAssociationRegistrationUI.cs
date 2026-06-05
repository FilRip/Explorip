using System.Runtime.InteropServices;

namespace ManagedShell.Common.Interfaces;

[ComImport()]
[Guid("1f76a169-f994-40ac-8fc8-0959e8874710")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IApplicationAssociationRegistrationUI
{
    void LaunchAdvancedAssociationUI([MarshalAs(UnmanagedType.LPWStr)] string pszFileExtension);
}

[ComImport()]
[Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
public class ApplicationAssociationRegistrationUI
{
}
