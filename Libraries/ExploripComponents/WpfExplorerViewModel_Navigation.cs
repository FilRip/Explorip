using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

    }

    [RelayCommand()]
    public void LostFocusEditPath(RoutedEventArgs e)
    {

    }

    [RelayCommand()]
    public void SearchTextKeyDown(KeyEventArgs e)
    {

    }

    [RelayCommand()]
    public void SearchButtonClick(RoutedEventArgs e)
    {

    }

    [RelayCommand()]
    public void PreviewMouseDown(MouseButtonEventArgs e)
    {

    }

    #endregion
}
