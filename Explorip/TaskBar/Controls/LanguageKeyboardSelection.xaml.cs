using System;
using System.Windows;
using System.Windows.Interop;

using Explorip.TaskBar.ViewModels;

using ExploripSharedCopy.Helpers;

using ExploripSharedCopy.WinAPI;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour LanguageKeyboardSelection.xaml
/// </summary>
public partial class LanguageKeyboardSelection : Window
{
    public LanguageKeyboardSelection(LanguageButtonViewModel button)
    {
        InitializeComponent();
        base.DataContext = button;
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        if (IsVisible)
            Close();
    }

    public new LanguageButtonViewModel DataContext
    {
        get { return (LanguageButtonViewModel)base.DataContext; }
    }

    private void Window_Activated(object sender, EventArgs e)
    {
        Top = DataContext.ParentTaskbar.Top - Height;
        Left = DataContext.ParentTaskbar.Width - Width;
    }

    private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        try
        {
            DataContext.SelectedItem = (LanguageViewModel)e.AddedItems[0];
        }
        catch (Exception) { /* Ignore errors */ }
    }
}
