using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class LanguageButtonViewModel : ObservableObject
{
    private LanguageKeyboardSelection _window;

    public LanguageButtonViewModel() : base()
    {
        _languagesList = [];
        IntPtr keyb = NativeMethods.GetKeyboardLayout(0);
        int lcid = keyb.ToInt32() & 0xFFFF;
        CultureInfo ci = CultureInfo.GetCultureInfo(lcid);
        _currentLanguage = ci.ThreeLetterWindowsLanguageName;
        RefreshListAvailableLanguages();
    }

    public Taskbar ParentTaskbar { get; set; }

    [ObservableProperty()]
    private string _currentLanguage;
    [ObservableProperty()]
    private ObservableCollection<LanguageViewModel> _languagesList;
    [ObservableProperty()]
    private LanguageViewModel _selectedItem;

    public void RefreshListAvailableLanguages()
    {
        LanguagesList.Clear();
        foreach (InputLanguage il in InputLanguage.InstalledInputLanguages)
            LanguagesList.Add(new LanguageViewModel(il));
    }

    [RelayCommand()]
    private void ShowLanguageLayout()
    {
        if (_window != null && _window.IsVisible)
            _window.Close();
        else
        {
            _window = new LanguageKeyboardSelection()
            {
                DataContext = this,
            };
            _window.Show();
        }
    }

    [RelayCommand()]
    private void MouseUp()
    {
        SelectedItem?.ActiveKeyboardLayout();
        _window?.Close();
    }
}
