using System;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace Explorip.Explorer.ViewModels;

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
    private bool _editMode = false;

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
        EditMode = true;
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

    #endregion

    public void ModifyValue(object newValue)
    {
        if (_parent?.CurrentKey != null)
            _parent.CurrentKey.SetValue(Name, newValue);
    }

    public void ModifyName(string newName)
    {
        if (_parent?.CurrentKey != null)
        {
            if (_parent.CurrentKey.GetValueNames().Contains(newName, StringComparer.OrdinalIgnoreCase))
                return;
            _parent.CurrentKey.DeleteValue(Name);
            _parent.CurrentKey.SetValue(newName, Value, Type);
        }
    }
}
