using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class SearchButtonViewModel : ObservableObject
{
    [RelayCommand()]
    private void ShowSearch()
    {
        ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_S);
        Stopwatch stopwatch = Stopwatch.StartNew();
        IntPtr ptrForegroundWindow = IntPtr.Zero;
        while (stopwatch.ElapsedMilliseconds < 2000)
        {
            Thread.Sleep(10);
            ptrForegroundWindow = NativeMethods.GetForegroundWindow();
            StringBuilder sb = new(256);
            NativeMethods.GetClassName(ptrForegroundWindow, sb, 256);
            if (sb.ToString() == "Windows.UI.Core.CoreWindow")
                break;
        }
        stopwatch.Stop();
        if (stopwatch.ElapsedMilliseconds >= 2000)
            return;
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        Taskbar currentTaskbar = ((MyTaskbarApp)Application.Current).ListAllTaskbar().FirstOrDefault(t => t.ScreenName == screen.DeviceName.TrimStart('.', '\\'));
        if (currentTaskbar != null)
        {
            NativeMethods.GetWindowRect(ptrForegroundWindow, out NativeMethods.Rect rect);
            NativeMethods.SetWindowPos(ptrForegroundWindow, IntPtr.Zero, (int)currentTaskbar.Left, (int)(currentTaskbar.Top * screen.ScaleFactor) - rect.Height, rect.Width, rect.Height, NativeMethods.SWP.SWP_SHOWWINDOW);
        }
    }
}
