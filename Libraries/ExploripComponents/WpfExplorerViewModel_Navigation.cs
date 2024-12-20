﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ExploripComponents;

public partial class WpfExplorerViewModel
{
    #region Fields property

    [ObservableProperty()]
    private bool _showSuggestions;
    [ObservableProperty()]
    private bool _modeEditPath;
    [ObservableProperty()]
    private bool _modeSearch;
    [ObservableProperty()]
    private string? _searchTo;
    [ObservableProperty()]
    private string? _editPath;
    [ObservableProperty()]
    private string? _comboBoxEditPath;

    #endregion

    #region Fields

    private readonly List<string> _navigationItems = [];
    private int _currentPosition = -1;

    #endregion

    #region Events

    public event EventHandler? ChangePath;

    #endregion

    #region Properties

    public bool AllowNavigatePrevious
    {
        get { return _currentPosition > 0; }
    }

    public bool AllowNavigateNext
    {
        get { return _currentPosition < _navigationItems.Count - 1; }
    }

    #endregion

    private void RefreshNavigation()
    {
        OnPropertyChanged(nameof(AllowNavigatePrevious));
        OnPropertyChanged(nameof(AllowNavigateNext));
    }

    #region Relay commands

    [RelayCommand()]
    public void NavigatePrevious(RoutedEventArgs e)
    {
        if (AllowNavigatePrevious)
        {
            _currentPosition--;
            BrowseTo(_navigationItems[_currentPosition], false);
        }
    }

    [RelayCommand()]
    public void NavigateNext(RoutedEventArgs e)
    {
        if (AllowNavigateNext)
        {
            _currentPosition++;
            BrowseTo(_navigationItems[_currentPosition], false);
        }
    }

    [RelayCommand()]
    public void EditPathKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            try
            {
                if (Directory.Exists(EditPath))
                    BrowseTo(EditPath);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    [RelayCommand()]
    public void LostFocusEditPath(RoutedEventArgs e)
    {
        ModeEditPath = false;
    }

    [RelayCommand()]
    public void SearchTextKeyDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void SearchButtonClick(RoutedEventArgs e)
    {
        ModeEditPath = false;
        ModeSearch = true;
        CurrentControl.SearchText.Focus();
    }

    [RelayCommand()]
    public void EditPathPreviewMouseDown(MouseButtonEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(CurrentControl.EditPath, e.GetPosition(CurrentControl.EditPath));
        if (result?.VisualHit is Border)
        {
            if (CurrentControl.EditPath.Template.FindName("PART_EditableTextBox", CurrentControl.EditPath) is not TextBox comboTextBoxChild)
                return;
            comboTextBoxChild.CaretIndex = comboTextBoxChild.Text.Length;
            if (e.ClickCount == 2)
                try
                {
                    comboTextBoxChild.SelectionStart = CurrentControl.EditPath.Text.LastIndexOf(@"\") + 1;
                    comboTextBoxChild.SelectionLength = CurrentControl.EditPath.Text.Length - comboTextBoxChild.SelectionStart;
                }
                catch (Exception) { /* Ignoring errors */ }
            e.Handled = true;
        }
    }

    [RelayCommand()]
    public void ChangeEditPath(MouseEventArgs e)
    {
        ModeEditPath = true;
        TextBox txt = (TextBox)CurrentControl.EditPath.Template.FindName("PART_EditableTextBox", CurrentControl.EditPath);
        txt.Focus();
    }

    [RelayCommand()]
    public void ModifyPath()
    {
        ChangeEditPath(new MouseEventArgs(Mouse.PrimaryDevice, 0));
    }

    [RelayCommand()]
    public void CopyCurrentAddress()
    {
        Clipboard.SetText(SelectedFolder?.FullPath ?? SelectedFolder?.DisplayText ?? "");
    }

    [RelayCommand()]
    public void DeleteHistory()
    {
        _currentPosition = 0;
        _navigationItems.Clear();
        _navigationItems.Add(SelectedFolder?.FullPath ?? "");
        RefreshNavigation();
    }

    #endregion
}
