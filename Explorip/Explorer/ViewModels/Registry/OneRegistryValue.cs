﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Exceptions;

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

    public string? DisplayValue
    {
        get
        {
            if (Type == RegistryValueKind.Binary)
            {
                byte[]? bin = (byte[])Value;
                if (bin != null && bin.Length > 0)
                {
                    StringBuilder sb = new();
                    foreach (byte b in bin)
                    {
                        if (sb.Length > 0)
                            sb.Append(' ');
                        sb.Append(b.ToString("X2"));
                    }
                    return sb.ToString();
                }
                return "";
            }
            return Value as string;
        }
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
    public void RenameMode()
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
            _parent.GetWriteKey().DeleteValue(Name);
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
                    _parent.GetWriteKey().SetValue(NewName, Value, Type);
                    _parent.GetWriteKey().DeleteValue(_name);
                    Name = NewName;
                    OnPropertyChanged(nameof(Name));
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
            RenameMode();
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

    [RelayCommand()]
    private void Rename()
    {
        RenameMode();
    }

    #endregion

    public void ModifyValue(object newValue)
    {
        if (_parent?.CurrentKey != null)
        {
            try
            {
                RegistryKey writeKey = _parent.GetWriteKey();
                switch (Type)
                {
                    case RegistryValueKind.Binary:
                        string[] splitter = newValue.ToString().Split(' ');
                        byte[] bytes = new byte[splitter.Length];
                        for (int i = 0; i < splitter.Length; i++)
                            bytes[i] = Convert.ToByte(splitter[i], 16);
                        writeKey.SetValue(Name, bytes);
                        break;
                    case RegistryValueKind.DWord:
                        if (!double.TryParse(newValue.ToString(), out double d) || d < int.MinValue || d > int.MaxValue)
                            throw new ExploripException(Constants.Localization.REGEDIT_ERROR_DWORD32);
                        writeKey.SetValue(Name, (int)d);
                        break;
                    case RegistryValueKind.QWord:
                        if (!double.TryParse(newValue.ToString(), out double ld) || ld < long.MinValue || ld > long.MaxValue)
                            throw new ExploripException(Constants.Localization.REGEDIT_ERROR_DWORD64);
                        writeKey.SetValue(Name, (long)ld);
                        break;
                    case RegistryValueKind.Unknown:
                        writeKey.SetValue(Name, newValue);
                        break;
                    case RegistryValueKind.None:
                        break;
                    default:
                        writeKey.SetValue(Name, newValue as string);
                        break;
                }
                Value = newValue;
                writeKey.Close();
            }
            catch (Exception ex)
            {
                _parent.GetRootParent().MainViewModel!.ErrorMessage = ex.Message;
                _parent.GetRootParent().MainViewModel!.ErrorVisible = true;
            }
        }
        if (_parent != null)
            _parent.GetRootParent().MainViewModel!.ShowModifyValue = false;
    }
}
