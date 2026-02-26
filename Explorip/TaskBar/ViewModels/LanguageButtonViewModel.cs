using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class LanguageButtonViewModel : ObservableObject, IDisposable
{
    private LanguageKeyboardSelection _window;
    private IntPtr _keyboardHookPtr;

    public LanguageButtonViewModel() : base()
    {
        _languagesList = [];
        RefreshAllLanguages();
        Models.HookRefreshLanguageLayout.Hook();
    }

    public void RefreshAllLanguages()
    {
        RefreshCurrentLayout();
        RefreshListAvailableLanguages();
    }

    public Taskbar ParentTaskbar { get; set; }

    [ObservableProperty()]
    private string _currentLanguage;
    [ObservableProperty()]
    private ObservableCollection<LanguageViewModel> _languagesList;
    [ObservableProperty()]
    private LanguageViewModel _selectedItem;
    private bool disposedValue;

    public void RefreshListAvailableLanguages()
    {
        LanguagesList.Clear();
        foreach (InputLanguage il in InputLanguage.InstalledInputLanguages)
            LanguagesList.Add(new LanguageViewModel(il));
    }

    public void RefreshCurrentLayout()
    {
        System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
        {
            IntPtr keyb = NativeMethods.GetKeyboardLayout(0);
            int lcid = keyb.ToInt32() & 0xFFFF;
            CultureInfo ci = CultureInfo.GetCultureInfo(lcid);
            CurrentLanguage = ci.ThreeLetterISOLanguageName.ToUpper();
        });
    }

    [RelayCommand()]
    private void ShowLanguageLayout()
    {
        if (_window != null && _window.IsVisible)
            _window.Close();
        else
        {
            _window = new LanguageKeyboardSelection(this);
            _window.Show();
        }
    }

    [RelayCommand()]
    private void MouseUp()
    {
        SelectedItem?.ActiveKeyboardLayout();
        RefreshCurrentLayout();
        _window?.Close();
    }

    #region IDisposable

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_keyboardHookPtr != IntPtr.Zero)
                {
                    NativeMethods.UnhookWindowsHookEx(_keyboardHookPtr);
                    _keyboardHookPtr = IntPtr.Zero;
                }
                _window?.Close();
                LanguagesList.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
