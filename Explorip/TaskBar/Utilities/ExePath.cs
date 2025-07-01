using System;
using System.Text;

using ManagedShell.Interop;

namespace Explorip.TaskBar.Utilities;

internal static class ExePath
{
    internal static string GetExecutablePath()
    {
        StringBuilder sb = new(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
        NativeMethods.GetModuleFileName(IntPtr.Zero, sb, ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
        return sb.ToString();
    }
}
