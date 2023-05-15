using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

using Explorip.WinAPI.Modeles;

using ManagedShell.Common.Helpers;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class TabTipButton : UserControl
    {
        public TabTipButton()
        {
            InitializeComponent();
        }

        private void StartKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (osk.IsChecked)
            {
                ShellHelper.ShellKeyCombo(VK.LWIN, VK.LCONTROL, VK.KEY_O);
            }
            else
            {
                try
                {
                    UIHostNoLaunch uiHostNoLaunch = new();
                    ((ITipInvocation)uiHostNoLaunch).Toggle(WinAPI.User32.GetDesktopWindow());
                    Marshal.ReleaseComObject(uiHostNoLaunch);
                }
                catch (Exception ex)
                {
                    if (ex.HResult == unchecked((int)0x80040154))
                    {
                        ProcessStartInfo psi = new()
                        {
                            UseShellExecute = true,
                            FileName = Path.Combine(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)), "Microsoft Shared", "Ink", "TabTip.exe"),
                        };
                        Process.Start(psi);
                    }
                }
            }
        }

        private void Osk_Click(object sender, RoutedEventArgs e)
        {
            osk.IsChecked = true;
            tabtip.IsChecked = false;
        }

        private void Tabtip_Click(object sender, RoutedEventArgs e)
        {
            osk.IsChecked = false;
            tabtip.IsChecked = true;
        }
    }
}
