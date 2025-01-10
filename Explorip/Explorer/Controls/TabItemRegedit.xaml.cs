using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemRegedit.xaml
/// </summary>
public partial class TabItemRegedit : TabItemExplorip
{
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
}
