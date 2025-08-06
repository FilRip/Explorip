using System;
using System.Linq;
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
        IntPtr ptrSearchWindow = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.SEARCH);
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        Taskbar currentTaskbar = ((MyTaskbarApp)Application.Current).ListAllTaskbar().FirstOrDefault(t => t.NumScreen == screen.DisplayNumber);
        if (currentTaskbar != null)
        {
            NativeMethods.GetWindowRect(ptrSearchWindow, out NativeMethods.Rect rect);
            NativeMethods.SetWindowPos(ptrSearchWindow, IntPtr.Zero, (int)(currentTaskbar.Left * screen.ScaleFactor), (int)(currentTaskbar.Top * screen.ScaleFactor) - rect.Height, rect.Width, rect.Height, NativeMethods.SWP.SWP_SHOWWINDOW);
        }
    }
}
