using System;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class LanguageViewModel(System.Windows.Forms.InputLanguage il) : ObservableObject
{
    private readonly IntPtr _handle = il.Handle;

    [ObservableProperty()]
    private string _shortName = il.Culture.ThreeLetterISOLanguageName.ToUpper();
    [ObservableProperty()]
    private string _fullName = il.Culture.DisplayName;
    [ObservableProperty()]
    private string _details = il.LayoutName;

    public void ActiveKeyboardLayout()
    {
        Debug.WriteLine($"Active keyboard {ShortName}");
        NativeMethods.ActivateKeyboardLayout(_handle, 0);
    }
}
