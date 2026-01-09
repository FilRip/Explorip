using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.ViewModels.Registry;
using Explorip.Helpers;

using Microsoft.Win32;

namespace Explorip.Explorer.ViewModels;

#nullable enable

public partial class TabItemRegeditViewModel : TabItemExploripViewModel, IDisposable
{
    #region Fields

    private readonly List<string> _historic;
    private int _currentNavigation;
    private bool disposedValue;
    private OneRegistryValue? _currentValueChange;
    private readonly TabItemRegedit _currentControl;
    private bool _forceBinary;

    #endregion

    #region Property fields

    [ObservableProperty()]
    private bool _modeEdit;
    [ObservableProperty()]
    private bool _modeSearch;
    [ObservableProperty()]
    private string? _searchTo;
    [ObservableProperty()]
    private string? _editPath;
    [ObservableProperty()]
    private string[]? _comboBoxEditPath;
    [ObservableProperty()]
    private bool _showSuggestions;
    [ObservableProperty()]
    private string? _errorMessage;
    [ObservableProperty()]
    private bool _errorVisible;
    [ObservableProperty()]
    private OneRegistryKey? _currentSelectedKey;
    [ObservableProperty()]
    private List<OneRegistryValue> _listViewItems;
    [ObservableProperty()]
    private List<OneRegistryKey> _regKeyItems;
    [ObservableProperty()]
    private bool _showModifyValue = false;
    [ObservableProperty()]
    private string _newValue = "";

    #endregion

    #region Constructors

    public TabItemRegeditViewModel(TabItemRegedit control) : base()
    {
        _currentControl = control;
        _historic = [];
        _currentNavigation = 0;
        _listViewItems = [];
        _regKeyItems = [];
        CreateBaseLocalRegistry();
    }

    #endregion

    #region Properties

    public TabItemRegedit CurrentControl
    {
        get { return _currentControl; }
    }

    public OneRegistryValue? CurrentValueChange
    {
        get { return _currentValueChange; }
    }

    public OneRegistryValue? ValueKeySelected { get; set; }

    public bool AllowNavigatePrevious
    {
        get { return _currentNavigation > 0; }
    }

    public bool AllowNavigateNext
    {
        get { return _currentNavigation < _historic.Count - 1; }
    }

    public Brush ForegroundPrevious
    {
        get
        {
            if (AllowNavigatePrevious)
                return ExploripSharedCopy.Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

    public Brush ForegroundNext
    {
        get
        {
            if (AllowNavigateNext)
                return ExploripSharedCopy.Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

    public bool EditValueMultiLines
    {
        get { return _currentValueChange?.Type == RegistryValueKind.ExpandString || _currentValueChange?.Type == RegistryValueKind.MultiString; }
    }

    public bool EditValueBinary
    {
        get { return _forceBinary || _currentValueChange?.Type == RegistryValueKind.Binary; }
    }

    #endregion

    #region RelayCommand

    #region Navigation

    [RelayCommand()]
    public void GotoPrevious()
    {
        if (AllowNavigatePrevious)
        {
            _currentNavigation--;
            BrowseTo(_historic[_currentNavigation], false);
            RefreshNavigation();
        }
    }

    [RelayCommand()]
    public void GotoNext()
    {
        if (AllowNavigateNext)
        {
            _currentNavigation++;
            BrowseTo(_historic[_currentNavigation], false);
            RefreshNavigation();
        }
    }

    [RelayCommand()]
    public void CleanHistoric()
    {
        _currentNavigation = 0;
        _historic.Clear();
        _historic.Add(CurrentSelectedKey!.ToString());
        RefreshNavigation();
    }

    #endregion

    #region Edit path manually

    [RelayCommand()]
    public void EditPath_KeyDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void EditPath_PreviewMouseDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void EditPath_LostFocus()
    {
        // TODO
    }

    #endregion

    #region Search in registry

    [RelayCommand()]
    public void SearchText_KeyDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void SearchButton_Click()
    {
        // TODO
    }

    #endregion

    #region Modify key/value

    [RelayCommand()]
    public void ModifyValue()
    {
        if (!ShowModifyValue)
            return;
        _currentValueChange?.ModifyValue(NewValue);
    }

    [RelayCommand()]
    public void CancelModifyValue()
    {
        ShowModifyValue = false;
    }

    [RelayCommand()]
    public void KeyUpTV(KeyEventArgs e)
    {
        if (e.Key == Key.Escape &&
            CurrentSelectedKey?.EditKeyName == true)
        {
            CurrentSelectedKey.CancelRenameMode();
        }
        else if (e.Key == Key.F2 &&
            CurrentSelectedKey?.EditKeyName == false)
        {
            CurrentSelectedKey.RenameMode();
        }
        else if (e.Key == Key.F5 && CurrentSelectedKey != null)
        {
            CurrentSelectedKey.RefreshSubKey();
            CurrentSelectedKey.RefreshValues();
        }
    }

    [RelayCommand()]
    public void KeyUpLV(KeyEventArgs e)
    {
        if (e.Key == Key.Escape &&
            CurrentValueChange?.EditValueName == true)
        {
            CurrentValueChange.EditValueName = false;
        }
        else if (e.Key == Key.F5 && CurrentSelectedKey != null)
            CurrentSelectedKey.RefreshValues();
        else if (CurrentValueChange != null && !CurrentValueChange.EditValueName && !ShowModifyValue)
        {
            if (e.Key == Key.F2)
                CurrentValueChange.RenameMode();
            else if (e.Key == Key.Enter || e.Key == Key.Return)
                CurrentValueChange.Modify();
        }
    }

    [RelayCommand()]
    public void AddNewKey()
    {
        try
        {
            if (CurrentSelectedKey?.Parent == null)
                return;
            string name = Constants.Localization.REGEDIT_NEW_KEY_NAME;
            int i = 1;
            CurrentSelectedKey.IsExpanded = true;
            while (CurrentSelectedKey.Children.Any(k => k.DisplayText == name.Replace("%%u", i.ToString())))
            {
                i++;
            }
            string definitiveName = name.Replace("%%u", i.ToString());
            CurrentSelectedKey.GetWriteKey().CreateSubKey(definitiveName);
            CurrentSelectedKey.RefreshSubKey();
            foreach (OneRegistryKey key in CurrentSelectedKey.Children)
                if (key.DisplayText == definitiveName)
                {
                    CurrentSelectedKey = key;
                    CurrentSelectedKey.IsSelected = true;
                    CurrentSelectedKey.RenameMode();
                    break;
                }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorVisible = true;
        }
    }

    [RelayCommand()]
    private void EditValue_PreviewKeyDown(KeyEventArgs e)
    {
        string key = e.Key.ToString().ToUpper();
        if (EditValueBinary && (key.Length > 1 || !char.IsLetter(key, 0) || key[0] > 'F' || key[0] < 'A'))
            e.Handled = true;
    }

    private void CreateNewValue(RegistryValueKind type, object defaultValue)
    {
        if (ListViewItems == null || CurrentSelectedKey?.CurrentKey == null || CurrentSelectedKey?.Parent == null)
            return;
        string name = Constants.Localization.REGEDIT_NEW_VALUE_NAME;
        int i = 1;
        while (ListViewItems.Any(v => v.Name == name.Replace("%%u", i.ToString())))
        {
            i++;
        }
        try
        {
            CurrentSelectedKey.GetWriteKey().SetValue(name.Replace("%%u", i.ToString()), defaultValue, type);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorVisible = true;
        }
        CurrentSelectedKey.RefreshValues();
        foreach (OneRegistryValue value in ListViewItems)
            if (value.Name == name)
            {
                Keyboard.Focus(CurrentControl.ValueLV);
                value.RenameMode();
                break;
            }
    }

    [RelayCommand()]
    public void AddNewValueString()
    {
        CreateNewValue(RegistryValueKind.String, "");
    }

    [RelayCommand()]
    public void AddNewValueBinary()
    {
        CreateNewValue(RegistryValueKind.Binary, Array.Empty<byte>());
    }

    [RelayCommand()]
    public void AddNewValueDword32()
    {
        CreateNewValue(RegistryValueKind.DWord, 0);
    }

    [RelayCommand()]
    public void AddNewValueDword64()
    {
        CreateNewValue(RegistryValueKind.QWord, 0);
    }

    [RelayCommand()]
    public void AddNewValueMultipleString()
    {
        CreateNewValue(RegistryValueKind.MultiString, "");
    }

    [RelayCommand()]
    public void AddNewValueExtendedString()
    {
        CreateNewValue(RegistryValueKind.ExpandString, "");
    }

    #endregion

    #endregion

    public void BrowseTo(string registryKey, bool addToHistoric = true)
    {
        if (addToHistoric)
        {
            if (_historic[_currentNavigation] != registryKey)
            {
                _currentNavigation++;
                if (_historic.Count > _currentNavigation)
                    _historic.RemoveRange(_currentNavigation, _historic.Count - _currentNavigation);
                _historic.Add(registryKey);
            }
        }
        else
        {
            string[] splitter = registryKey.Split('\\');
            OneRegistryKey? currentKey = null;
            foreach (string key in splitter)
            {
                if (currentKey == null)
                    currentKey = RegKeyItems[0];
                else
                {
                    if (!currentKey.IsExpanded)
                        currentKey.IsExpanded = true;
                    currentKey = currentKey.Children.FirstOrDefault(k => k.DisplayText == key);
                }
            }
            currentKey?.IsSelected = true;
        }
        RefreshNavigation();
    }

    public void RefreshNavigation()
    {
        OnPropertyChanged(nameof(AllowNavigateNext));
        OnPropertyChanged(nameof(ForegroundNext));
        OnPropertyChanged(nameof(AllowNavigatePrevious));
        OnPropertyChanged(nameof(ForegroundPrevious));
    }

    private void CreateBaseLocalRegistry()
    {
        RegKeyItems.Add(new OneRegistryKey(null, null, null, displayText: Environment.SpecialFolder.MyComputer.RealName()) { MainViewModel = this, IsExpanded = true, IsSelected = true });
        CurrentSelectedKey = RegKeyItems[0];
        _historic.Add(CurrentSelectedKey.ToString());
    }

    public void SetModifyValue(OneRegistryValue keyToChange, bool forceBinary = false)
    {
        if (ShowModifyValue)
            return;
        _currentValueChange = keyToChange;
        NewValue = keyToChange.DisplayValue?.ToString() ?? "";
        ShowModifyValue = true;
        _forceBinary = forceBinary;
        OnPropertyChanged(nameof(CurrentValueChange));
        OnPropertyChanged(nameof(EditValueBinary));
        OnPropertyChanged(nameof(EditValueMultiLines));
    }

    #region Destructor

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
                _historic.Clear();
                CurrentValueChange?.EditValueName = false;
                _currentValueChange = null;
                foreach (OneRegistryKey registryKey in RegKeyItems)
                    registryKey.Dispose();
                RegKeyItems.Clear();
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
