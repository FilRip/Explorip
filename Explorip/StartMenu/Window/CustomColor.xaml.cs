using System.Windows.Interop;

using Explorip.TaskBar.ViewModels;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

namespace Explorip.StartMenu.Controls;

/// <summary>
/// Logique d'interaction pour CustomColor.xaml
/// </summary>
public partial class CustomColor : System.Windows.Window
{
    public CustomColor()
    {
        InitializeComponent();
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    public new CustomColorViewModel DataContext
    {
        get { return (CustomColorViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }
}
