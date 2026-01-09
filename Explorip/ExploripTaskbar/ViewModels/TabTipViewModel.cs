using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Interfaces;
using ManagedShell.Common.SupportingClasses;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.TaskBar.ViewModels;

public partial class TabTipViewModel : ObservableObject
{
    [ObservableProperty()]
    private bool _oskChecked;
    [ObservableProperty()]
    private bool _tabtipChecked;

    [RelayCommand()]
    private void StartKeyboard()
    {
        if (OskChecked)
        {
            ShellHelper.ShellKeyCombo(VK.LWIN, VK.LCONTROL, VK.KEY_O);
        }
        else
        {
            try
            {
                UIHostNoLaunch uiHostNoLaunch = new();
                ((ITipInvocation)uiHostNoLaunch).Toggle(GetDesktopWindow());
                Marshal.ReleaseComObject(uiHostNoLaunch);
            }
            catch (Exception ex)
            {
                if (ex.HResult == unchecked((int)CoolBytes.Helpers.Win32Error.REGDB_E_CLASSNOTREG))
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

    [RelayCommand()]
    private void OskClick()
    {
        OskChecked = true;
        TabtipChecked = false;
    }

    [RelayCommand()]
    private void TabtipClick()
    {
        OskChecked = false;
        TabtipChecked = true;
    }
}
