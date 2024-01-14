using System;
using System.Text;

namespace Explorip.TaskBar.Utilities;

static class ExePath
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

    internal static string GetExecutablePath()
    {
        StringBuilder sb = new(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
        GetModuleFileName(IntPtr.Zero, sb, ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
        return sb.ToString();
    }
}
