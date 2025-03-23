using System;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class LanguageViewModel(InputLanguage il) : ObservableObject
{
    private readonly IntPtr _handle = il.Handle;

    [ObservableProperty()]
    private string _shortName = il.Culture.ThreeLetterWindowsLanguageName;
    [ObservableProperty()]
    private string _fullName = il.Culture.NativeName;
    [ObservableProperty()]
    private string _details = il.LayoutName;

    public void ActiveThisKeyboard()
    {
        NativeMethods.ActivateKeyboardLayout(_handle, 0);
    }
}
