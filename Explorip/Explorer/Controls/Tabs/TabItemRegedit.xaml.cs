using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Explorer.ViewModels.Registry;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemRegedit.xaml
/// </summary>
public partial class TabItemRegedit : TabItemExplorip
{
    private Point _lastMousePos;
    private bool _moveSP = false;

    public TabItemRegedit()
    {
        InitializeComponent();
        InitializeExplorip();
        SetTitle(Constants.Localization.REGISTRY_EDITOR);
    }

    private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is Grid myGrid && myGrid.ColumnDefinitions.Count > 0)
        {
            int sizeCol = (int)(MainGrid.ColumnDefinitions[1].ActualWidth / 2) - 100;
            myGrid.ColumnDefinitions[0].Width = new GridLength(sizeCol);
            myGrid.ColumnDefinitions[1].Width = new GridLength(100);
            myGrid.ColumnDefinitions[2].Width = new GridLength(sizeCol);
        }
    }

    private void IsVisibleChanged_SetFocus(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Visibility == Visibility.Visible)
        {
            fe.Focus();
            FocusManager.SetFocusedElement(this, fe);
            Keyboard.Focus(fe);
            if (fe is TextBox txt)
                txt.SelectAll();
        }
    }

    private void TextBlock_MouseMove(object sender, MouseEventArgs e)
    {
        if (_moveSP)
        {
            Point mousePos = Mouse.GetPosition(this);
            Point mouseOffset = Mouse.GetPosition(SPModifyValue);
            double xDelta = _lastMousePos.X - mousePos.X - mouseOffset.X;
            double yDelta = _lastMousePos.Y - mousePos.Y - mouseOffset.Y;
            Thickness newMarging = SPModifyValue.Margin;
            newMarging.Left -= xDelta;
            newMarging.Top -= yDelta;
            SPModifyValue.Margin = newMarging;
            _lastMousePos = mousePos;
        }
    }

    private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _lastMousePos = Mouse.GetPosition(this);
        _moveSP = true;
    }

    private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _moveSP = false;
    }

#nullable enable

    private void TreeView_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key.ToString().Length == 1 && KeyTV.SelectedItem is OneRegistryKey current)
        {
            OneRegistryKey? find = FindNextOne(current, e.Key.ToString().ToLower());
            find ??= FindNextOne((OneRegistryKey)KeyTV.Items[0], e.Key.ToString().ToLower());

            if (find != null)
                find.IsSelected = true;
        }
    }

    private OneRegistryKey? FindNextOne(OneRegistryKey current, string name)
    {
        // Check the entry one
        if (!current.IsSelected && current.DisplayText.ToLower().StartsWith(name))
            return current;

        // Check if the entry one has opened child
        if (current.IsExpanded && current.Children.Count > 0)
        {
            OneRegistryKey? find = FindNextOne(current.Children[0], name);
            if (find != null)
                return find;
        }

        // Check next one in the same parent has the entry one
        if (current.Parent == null)
            return null;
        for (int i = current.Parent.Children.IndexOf(current) + 1; i < current.Parent.Children.Count; i++)
        {
            OneRegistryKey currentLoop = current.Parent.Children[i];
            if (currentLoop.DisplayText.ToLower().StartsWith(name))
                return currentLoop;
            if (currentLoop.IsExpanded)
                return FindNextOne(currentLoop, name);
        }

        // Check next parent
        if (current.Parent.Parent != null)
        {
            int next = current.Parent.Parent.Children.IndexOf(current.Parent) + 1;
            if (next < current.Parent.Parent.Children.Count)
                return FindNextOne(current.Parent.Parent.Children[next], name);
        }

        return null;
    }

    private void KeyTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Debug.WriteLine("Selected key : " + KeyTV.SelectedItem.ToString());
    }
}
