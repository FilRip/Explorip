using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper]
    internal class ApplicationView(ComInterfaceAssembly assembly, object comObject, string comInterfaceName = null) : ComInterfaceWrapperBase(assembly, comObject, comInterfaceName)
    {
        public string GetAppUserModelId()
        {
            object[] param = Args((string)null);
            this.Invoke(param);

            return (string)param[0];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public enum ApplicationViewCloakType
    {
        AVCT_NONE = 0,
        AVCT_DEFAULT = 1,
        AVCT_VIRTUAL_DESKTOP = 2
    }

    public enum ApplicationViewCompatibilityPolicy
    {
        AVCP_NONE = 0,
        AVCP_SMALL_SCREEN = 1,
        AVCP_TABLET_SMALL_SCREEN = 2,
        AVCP_VERY_SMALL_SCREEN = 3,
        AVCP_HIGH_SCALE_FACTOR = 4
    }
}
