using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Interop;

using Microsoft.Win32;

namespace Explorip.Explorer.ViewModels.Registry;

#nullable enable

public partial class OneRegistryValue(string name, RegistryValueKind type, object value, OneRegistryKey parent) : ObservableObject()
{
    #region Fields

    private readonly OneRegistryKey _parent = parent;
    private readonly Stopwatch _swRenameValue = Stopwatch.StartNew();
    private string _name = name;

    #endregion

    #region Fields Property

    [ObservableProperty()]
    private RegistryValueKind _type = type;
    [ObservableProperty()]
    private object _value = value;
    [ObservableProperty()]
    private string? _newName;
    [ObservableProperty()]
    private bool _editValueName = false;

    #endregion

    #region Properties

    public string Name
    {
        get { return (_name == "" ? Constants.Localization.REGEDIT_STRING_DEFAULT : _name); }
        set { _name = value; }
    }

    #endregion

    #region RelayCommand

    [RelayCommand()]
    public void Modify()
    {
        _parent.GetRootParent().MainViewModel!.SetModifyValue(this);
    }

    [RelayCommand()]
    public void ModifyBinary()
    {
        _parent.GetRootParent().MainViewModel!.SetModifyValue(this, true);
    }

    [RelayCommand()]
    public void Rename()
    {
        NewName = _name;
        EditValueName = true;
    }

    [RelayCommand()]
    public void Delete()
    {
        if (_parent?.CurrentKey != null &&
            MessageBox.Show(Constants.Localization.REGEDIT_CONFIRM_DELETE_VALUE, Constants.Localization.REGISTRY_EDITOR, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            _parent.CurrentKey.DeleteValue(Name);
            _parent.RefreshValues();
        }
    }

    [RelayCommand()]
    public void EditNameKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            NewName = _name;
            EditValueName = false;
            e.Handled = true;
        }
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            EditNameLostFocus();
            EditValueName = false;
            e.Handled = true;
        }
        else if (e.Key == Key.F2)
        {
            EditValueName = true;
            e.Handled = true;
        }
    }

    [RelayCommand()]
    public void EditNameLostFocus()
    {
        if (_parent?.CurrentKey != null)
        {
            if (_parent.CurrentKey.GetValueNames().Contains(NewName, StringComparer.OrdinalIgnoreCase))
                return;
            try
            {
                if (NewName != null && NewName != _name)
                {
                    _parent.CurrentKey.SetValue(NewName, Value, Type);
                    _parent.CurrentKey.DeleteValue(_name);
                    Name = NewName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand()]
    private void MouseDown(MouseButtonEventArgs e)
    {
        if (e.OriginalSource is TextBlock tb && tb.Name == "ValueName" &&
            _swRenameValue.ElapsedMilliseconds > NativeMethods.GetDoubleClickTime() && e.ChangedButton == MouseButton.Left &&
            _parent.GetRootParent().MainViewModel!.ValueKeySelected == this)
        {
            Rename();
            e.Handled = true;
            return;
        }
        _parent.GetRootParent().MainViewModel!.ValueKeySelected = this;
        _swRenameValue.Restart();
        if (!EditValueName && e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
        {
            Modify();
        }
    }

    #endregion

    public void ModifyValue(object newValue)
    {
        if (_parent?.CurrentKey != null)
        {
            _parent.CurrentKey.SetValue(Name, newValue);
            Value = newValue;
        }
    }
}
