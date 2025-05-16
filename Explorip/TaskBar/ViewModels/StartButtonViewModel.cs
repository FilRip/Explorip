using System;
using System.Windows.Input;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.StartMenu.Window;
using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class StartButtonViewModel : ObservableObject
{
    private readonly bool _replaceStartMenu;
    private bool allowOpenStart;
    private DispatcherTimer pendingOpenTimer;

    public StartButton ParentControl { get; set; }

    public StartButtonViewModel()
    {
        _replaceStartMenu = ConfigManager.TaskbarReplaceStartMenu;
    }

    public void Init()
    {
        pendingOpenTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = new TimeSpan(0, 0, 0, 1)
        };
        pendingOpenTimer.Tick += (sender, args) =>
        {
            try
            {
                // if the start menu didn't open, flip the button back to unchecked
                ParentControl.Start.IsChecked = false;
                pendingOpenTimer.Stop();
            }
            catch (Exception) { /* Ignore errors */ }
        };
    }

    public void SetStartMenuState(bool opened)
    {
        ParentControl.Dispatcher.Invoke(() =>
        {
            ParentControl.Start.IsChecked = opened;
        });
        pendingOpenTimer.Stop();
    }

    [RelayCommand()]
    private void StartOnClick()
    {
        if (_replaceStartMenu)
        {
            if (StartMenuWindow.MyStartMenu.IsVisible)
                StartMenuWindow.MyStartMenu.Hide();
            else
            {
                StartMenuWindow.MyStartMenu.Show();
                StartMenuWindow.MyStartMenu.Activate();
            }
        }
        else
        {
            if (allowOpenStart)
            {
                pendingOpenTimer.Start();
                try
                {
                    IntPtr ptrStartMenuWindow = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.START);
                    if (ptrStartMenuWindow != IntPtr.Zero)
                    {
                        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
                        NativeMethods.SetWindowPos(ptrStartMenuWindow, IntPtr.Zero, (int)screen.WorkingArea.X, (int)screen.WorkingArea.Y, (int)screen.WorkingArea.Width, (int)screen.WorkingArea.Height, NativeMethods.SWP.SWP_NOACTIVATE);
                    }
                    ShellHelper.ShowStartMenu();
                }
                catch (Exception) { /* Ignore errors */ }

                return;
            }

            ParentControl.Start.IsChecked = false;
        }
    }

    [RelayCommand()]
    private void PreviewMouseLeftButtonDown()
    {
        allowOpenStart = (ParentControl.Start.IsChecked == false);
    }

    [RelayCommand()]
    private void MouseRightButtonUp(MouseButtonEventArgs e)
    {
        if (EnvironmentHelper.IsWindows10OrBetter)
        {
            ShellHelper.ShowStartContextMenu();
            e.Handled = true;
        }
    }

    public void AppVisibilityHelper_StartMenuVisibilityChanged(object sender, ManagedShell.Common.SupportingClasses.LauncherVisibilityEventArgs e)
    {
        SetStartMenuState(e.Visible);
    }
}
