using System.Linq;

using Explorip.Explorer.ViewModels;
using Explorip.Explorer.ViewModels.Sftp;

namespace Explorip.Explorer.Controls.Tabs;

/// <summary>
/// Logique d'interaction pour TabItemSftp.xaml
/// </summary>
public partial class TabItemSftp : TabItemExplorip
{
    public TabItemSftp()
    {
        InitializeComponent();
    }

    public new TabItemSftpViewModel DataContext
    {
        get { return (TabItemSftpViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        DataContext.ListSelected.Clear();
        DataContext.ListSelected.AddRange(LVItems.SelectedItems.OfType<SftpItem>());
    }

    private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        DataContext.SelectedFolder = (SftpFolder)TVItems.SelectedItem;
    }

    private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        DataContext.Password = PasswordTextBox.Password;
    }

    private void TreeItemGrid_Drop(object sender, System.Windows.DragEventArgs e)
    {
    }

    private void AllowDrop_DragOver(object sender, System.Windows.DragEventArgs e)
    {
    }

    private void AllowDrop_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
    {
    }

    private void Grid_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
    }

    private void Grid_DragLeave(object sender, System.Windows.DragEventArgs e)
    {
    }

    private void EditBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }

    private void EditNameBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
    }
}
