using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class LanguageButtonViewModel : ObservableObject
{
    public LanguageButtonViewModel() : base()
    {
        _languagesList = [];
        IntPtr keyb = NativeMethods.GetKeyboardLayout(0);
        int lcid = keyb.ToInt32() & 0xFFFF;
        CultureInfo ci = CultureInfo.GetCultureInfo(lcid);
        _currentLanguage = ci.ThreeLetterWindowsLanguageName;
        RefreshListAvailableLanguages();
    }

    [ObservableProperty()]
    private string _currentLanguage;
    [ObservableProperty()]
    private ObservableCollection<LanguageViewModel> _languagesList;

    public void RefreshListAvailableLanguages()
    {
        LanguagesList.Clear();
        foreach (InputLanguage il in InputLanguage.InstalledInputLanguages)
            LanguagesList.Add(new LanguageViewModel(il));
    }
}
