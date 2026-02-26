using System.Windows;
using System.Windows.Input;

using ExploripCopy.Models;
using ExploripCopy.ViewModels;

using WpfScreenHelper;

namespace ExploripCopy.GUI;

/// <summary>
/// Logique d'interaction pour ChoiceConflictFiles.xaml
/// </summary>
public partial class ChoiceConflictFiles : Window
{
    public ChoiceConflictFiles()
    {
        InitializeComponent();
        Icon = Constants.Icons.MainIconSource;
        Screen screen = MouseHelper.MouseScreen;
        if (screen != null)
            WindowScreenHelper.SetCenterOnScreen(this, screen);
    }

    public new ChoiceOnCollisionViewModel DataContext
    {
        get { return (ChoiceOnCollisionViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source == ReplaceAll)
            DataContext.Choice = EChoiceFileOperation.ReplaceAll;
        else if (e.Source == IgnoreAll)
            DataContext.Choice = EChoiceFileOperation.KeepExisting;
        else if (e.Source == IgnoreSameDateSize)
            DataContext.Choice = EChoiceFileOperation.KeepMostRecent;
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
    {
        DataContext.Choice = EChoiceFileOperation.None;
        Close();
    }

    private bool _startDrag;
    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (_startDrag && WindowState != WindowState.Minimized && IsVisible && IsActive)
        {
            _startDrag = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }

    private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _startDrag = true;
    }

    private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _startDrag = false;
    }
}
