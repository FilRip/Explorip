using System;
using System.Runtime.InteropServices;

namespace Explorip;

internal static class ConsoleHelper
{
    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole(int dwProcessId);

    public static void WriteToConsole()
    {
        AttachConsole(-1);
        Console.WriteLine();
        Console.WriteLine("List of command line :");
        Console.WriteLine("taskbar             : Launch the taskbar, on main screen (only, if you have multi-monitors)");
        Console.WriteLine("taskbars            : Launch taskbar on all screens (if you have multi-monitors)");
        Console.WriteLine("desktop             : Launch the desktop replacement, on main screen (only, if you have multi-monitors)");
        Console.WriteLine("desktops            : Launch the desktop replacement on all screens (if you have multi-monitors)");
        Console.WriteLine("startmenu           : Launch the Start menu replacement");
        Console.WriteLine("newinstance         : Launch a new instance of file explorer");
        Console.WriteLine("donotcheckforupdate : Do not check for online new version of Explorip");
        Console.WriteLine("tracemem            : Show, on bottom right of tasbar, the memory used by this process in real time");
        Console.WriteLine("disableWriteConfig  : Disable change of current settings");
        Console.WriteLine("WithoutHook         : Disable the Explorip File Operation Intercepter, when launch file explorer (some anti-virus report it as a virus)");
        Console.WriteLine("UseOwnCopier        : The opposite of DisableHook, force to launch Explorip FIle Operation Intercepter");
        Console.WriteLine("update              : Check for online update of Explorip before launch");
        Console.WriteLine("");
        Console.WriteLine("No command line mean : launch a File Explorer, with Desktop as root/current directory");
        Console.WriteLine("");
        Console.WriteLine("You can specify any folder path too, to launch file explorer on this folder");
        FreeConsole(-1);
    }
}
