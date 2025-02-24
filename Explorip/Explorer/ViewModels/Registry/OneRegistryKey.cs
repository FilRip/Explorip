using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;

using Microsoft.Win32;

namespace Explorip.Explorer.ViewModels.Registry;

#nullable enable

public partial class OneRegistryKey : ObservableObject, IDisposable
{
    #region Fields

    private readonly string? _machineName;
    private readonly RegistryHive? _hive;
    private readonly string? _key;
    private string? _fullPath;
    private static readonly OneRegistryKey _dummyKey = new(null, null, null);
    private readonly RegistryKey? _currentRegistryKey;
    private readonly string? _errorMessage;
    private bool disposedValue;

    #endregion

    #region Fields property

    [ObservableProperty()]
    private string _displayText;
    [ObservableProperty()]
    private ObservableCollection<OneRegistryKey> _children;
    [ObservableProperty()]
    private bool _isExpanded;
    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private bool _editKeyName;
    [ObservableProperty()]
    private string? _newKeyName;

    #endregion

    #region Constructors/Destructors

    public OneRegistryKey(OneRegistryKey? parentKey, RegistryHive? hive, string? subKey, string? machineName = null, string? displayText = null) : base()
    {
        Parent = parentKey;
        _children = [];
        _machineName = machineName;
        _hive = hive;
        _key = subKey?.Trim('\\');
        _editKeyName = false;
        if (displayText == null)
        {
            if (!string.IsNullOrWhiteSpace(subKey))
            {
                string[] fullKey = subKey!.Split('\\');
                _displayText = fullKey[fullKey.Length - 1];
            }
            else if (_hive != null)
                _displayText = _hive!.Value.RealName();
            else
                _displayText = "ERROR ?";
        }
        else
            _displayText = displayText;

        if (_hive != null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_machineName))
                    _currentRegistryKey = RegistryKey.OpenBaseKey(_hive.Value, RegistryView.Default);
                else
                    _currentRegistryKey = RegistryKey.OpenRemoteBaseKey(_hive.Value, _machineName, RegistryView.Default);
                if (!string.IsNullOrWhiteSpace(_key))
                    _currentRegistryKey = _currentRegistryKey.OpenSubKey(_key);
                if (_currentRegistryKey.SubKeyCount > 0)
                    _children.Add(_dummyKey);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _currentRegistryKey?.Close();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void DestroyAllChild()
    {
        for (int i = Children.Count - 1; i >= 0; i--)
            Children[i].Dispose();
        Children.Clear();
    }

    #endregion

    public void RefreshSubKey()
    {
        DestroyAllChild();
        if (_hive == null)
        {
            Children.Add(new OneRegistryKey(this, RegistryHive.ClassesRoot, "", _machineName));
            Children.Add(new OneRegistryKey(this, RegistryHive.CurrentUser, "", _machineName));
            Children.Add(new OneRegistryKey(this, RegistryHive.LocalMachine, "", _machineName));
            Children.Add(new OneRegistryKey(this, RegistryHive.Users, "", _machineName));
            Children.Add(new OneRegistryKey(this, RegistryHive.CurrentConfig, "", _machineName));
        }
        else if (_currentRegistryKey != null)
        {
            string[] listSubKey;
            List<OneRegistryKey> items = [];
            listSubKey = _currentRegistryKey.GetSubKeyNames();
            foreach (string key in listSubKey.OrderBy(name => name))
                items.Add(new OneRegistryKey(this, _hive, _key + @"\" + key));
            Children = [.. items];
        }
    }

    public OneRegistryKey GetRootParent()
    {
        OneRegistryKey parent = this;
        while (parent.Parent != null)
        {
            parent = parent.Parent;
        }
        return parent;
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded && (Children.Count == 0 || Children[0] == _dummyKey))
            RefreshSubKey();
    }

    partial void OnIsSelectedChanged(bool value)
    {
        if (IsSelected)
        {
            TabItemRegeditViewModel _mainViewModel = GetRootParent().MainViewModel!;
            if (_currentRegistryKey == null)
            {
                _mainViewModel.ListViewItems.Clear();
                return;
            }
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                _mainViewModel.ErrorMessage = _errorMessage;
                _mainViewModel.ErrorVisible = true;
            }
            else
                _mainViewModel.ErrorVisible = false;
            RefreshValues();
            _mainViewModel.CurrentSelectedKey = this;
            _mainViewModel.BrowseTo(this.ToString());
        }
    }

    public void RefreshValues()
    {
        List<OneRegistryValue> items = [];
        bool defaultAlreadyExist = false;
        foreach (string valueName in _currentRegistryKey!.GetValueNames().OrderBy(name => name))
        {
            if (valueName == "")
                defaultAlreadyExist = true;
            items.Add(new OneRegistryValue(valueName, _currentRegistryKey.GetValueKind(valueName), _currentRegistryKey.GetValue(valueName), this));
        }
        // Special case, the default value of this key
        if (!defaultAlreadyExist)
            items.Insert(0, new OneRegistryValue("", RegistryValueKind.String, _currentRegistryKey!.GetValue(""), this));
        GetRootParent().MainViewModel!.ListViewItems = items;
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(_fullPath))
        {
            OneRegistryKey parent = GetRootParent();
            _fullPath = parent.MachineName ?? Environment.SpecialFolder.MyComputer.RealName();
            if (_hive != null)
                _fullPath += @"\" + _hive!.Value.RealName();
            if (!string.IsNullOrWhiteSpace(_key))
                _fullPath += @"\" + _key;
        }
        return _fullPath!;
    }

    public void CreateSubKey(string name)
    {
        RegistryKey key = GetWriteKey();
        key.CreateSubKey(name);
        key.Close();
    }

    public RegistryKey GetWriteKey()
    {
        return Parent!.CurrentKey!.OpenSubKey(DisplayText, true);
    }

    public void RenameMode()
    {
        NewKeyName = DisplayText;
        EditKeyName = true;
    }

    public void CancelRenameMode()
    {
        NewKeyName = DisplayText;
        EditKeyName = false;
    }

    #region RelayCommand

    [RelayCommand()]
    private void VisibleEditMode()
    {
        NewKeyName = DisplayText;
    }

    [RelayCommand()]
    private void NewNameKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            CancelRenameMode();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            NewNameLostFocus();
            EditKeyName = false;
            e.Handled = true;
        }
        else if (e.Key == Key.F2 && !EditKeyName)
        {
            EditKeyName = true;
            e.Handled = true;
        }
    }

    [RelayCommand()]
    private void NewNameLostFocus()
    {
        if (NewKeyName != DisplayText)
        {
            try
            {
                Parent!.CurrentKey.RenameSubKey(DisplayText, NewKeyName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand()]
    private void DeleteSubKey()
    {
        if (MessageBox.Show(Constants.Localization.REGEDIT_CONFIRM_DELETE_KEY, Constants.Localization.REGISTRY_EDITOR, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                Parent!.CurrentKey!.DeleteSubKeyTree(DisplayText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #endregion

    #region Properties

    public OneRegistryKey? Parent { get; set; }

    public TabItemRegeditViewModel? MainViewModel { get; set; }

    public string? MachineName
    {
        get { return _machineName; }
    }

    public RegistryKey? CurrentKey
    {
        get { return _currentRegistryKey; }
    }

    public string? CurrentPath
    {
        get { return _key; }
    }

    #endregion
}
