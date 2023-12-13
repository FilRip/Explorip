using System.Windows;
using System.Windows.Interop;

using Explorip.Helpers;
using Explorip.WinAPI;

namespace Explorip.Explorer.Controls
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class CreateShortcutWindow : Window
    {
        public CreateShortcutWindow()
        {
            InitializeComponent();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
            Title = Constants.Localization.CREATE_SHORTCUT;
        }
    }
}
