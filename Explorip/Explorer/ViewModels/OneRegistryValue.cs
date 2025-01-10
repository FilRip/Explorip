using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace Explorip.Explorer.ViewModels;

#nullable enable

public partial class OneRegistryValue(string name, RegistryValueKind type, object value, OneRegistryKey parent) : ObservableObject()
{
    #region Fields

    private readonly OneRegistryKey _parent = parent;

    #endregion

    #region Fields Property

    [ObservableProperty()]
    private RegistryValueKind _type = type;
    [ObservableProperty()]
    private object _value = value;
    [ObservableProperty()]
    private string _name = name;
    [ObservableProperty()]
    private string? _newName;
    [ObservableProperty()]
    private bool _editValueName = false;

    #endregion

    #region RelayCommand

    [RelayCommand()]
    public void Modify()
    {
        // TODO
    }

    [RelayCommand()]
    public void ModifyBinary()
    {
        // TODO
    }

    [RelayCommand()]
    public void Rename()
    {
        NewName = Name;
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
            NewName = Name;
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
                _parent.CurrentKey.SetValue(NewName, Value, Type);
                _parent.CurrentKey.DeleteValue(Name);
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
        if (!EditValueName && e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
        {
            MessageBox.Show("ok");
        }
    }

    #endregion

    public void ModifyValue(object newValue)
    {
        if (_parent?.CurrentKey != null)
            _parent.CurrentKey.SetValue(Name, newValue);
    }
}
