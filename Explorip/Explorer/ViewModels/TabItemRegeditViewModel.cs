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

    private readonly List<OneRegistryKey> _historic;
    private int _currentNavigation;
    private bool disposedValue;
    private OneRegistryValue? _currentValueChange;
    private readonly TabItemRegedit _currentControl;

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
        get { return _currentValueChange!.Type == RegistryValueKind.ExpandString || _currentValueChange!.Type == RegistryValueKind.MultiString; }
    }

    public bool EditValueBinary
    {
        get { return _currentValueChange!.Type == RegistryValueKind.Binary; }
    }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public string TabTitle
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        get { return Constants.Localization.REGISTRY_EDITOR; }
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
        _historic.Add(CurrentSelectedKey!);
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
    }

    [RelayCommand()]
    public void AddNewKey()
    {
        try
        {
            string name = Constants.Localization.REGEDIT_NEW_KEY_NAME;
            int i = 1;
            CurrentSelectedKey!.IsExpanded = true;
            while (CurrentSelectedKey.Children.Any(k => k.DisplayText == name.Replace("%%u", i.ToString())))
            {
                i++;
            }
            string definitiveName = name.Replace("%%u", i.ToString());
            CurrentSelectedKey.CreateSubKey(definitiveName);
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

    private void CreateNewValue(RegistryValueKind type)
    {
        if (ListViewItems == null || CurrentSelectedKey?.CurrentKey == null)
            return;
        string name = Constants.Localization.REGEDIT_NEW_VALUE_NAME;
        int i = 1;
        while (ListViewItems.Any(v => v.Name == name.Replace("%%u", i.ToString())))
        {
            i++;
        }
        try
        {
            CurrentSelectedKey.CurrentKey.SetValue(name, null, type);
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
        CreateNewValue(RegistryValueKind.String);
    }

    [RelayCommand()]
    public void AddNewValueBinary()
    {
        CreateNewValue(RegistryValueKind.Binary);
    }

    [RelayCommand()]
    public void AddNewValueDword32()
    {
        CreateNewValue(RegistryValueKind.DWord);
    }

    [RelayCommand()]
    public void AddNewValueDword64()
    {
        CreateNewValue(RegistryValueKind.QWord);
    }

    [RelayCommand()]
    public void AddNewMultipleString()
    {
        CreateNewValue(RegistryValueKind.MultiString);
    }

    [RelayCommand()]
    public void AddNewExtendedString()
    {
        CreateNewValue(RegistryValueKind.ExpandString);
    }

    #endregion

    #endregion

    private void BrowseTo(OneRegistryKey registryKey, bool addToHistoric = true)
    {
        // TODO
        if (addToHistoric)
            _historic.Add(registryKey);
        _currentNavigation++;
        RefreshNavigation();
    }

    public void RefreshNavigation()
    {
        OnPropertyChanged(nameof(AllowNavigateNext));
        OnPropertyChanged(nameof(AllowNavigatePrevious));
    }

    private void CreateBaseLocalRegistry()
    {
        RegKeyItems.Add(new OneRegistryKey(null, null, null, displayText: Environment.SpecialFolder.MyComputer.RealName()) { MainViewModel = this, IsExpanded = true, IsSelected = true });
        CurrentSelectedKey = RegKeyItems[0];
    }

    public void SetModifyValue(OneRegistryValue keyToChange)
    {
        if (ShowModifyValue)
            return;
        _currentValueChange = keyToChange;
        NewValue = keyToChange.Value.ToString();
        ShowModifyValue = true;
        OnPropertyChanged(nameof(CurrentValueChange));
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
                // TODO: supprimer l'état managé (objets managés)
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
