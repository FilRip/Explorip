using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemFileSystemManager.xaml
/// </summary>
public partial class TabItemFileSystemManagement : TabItemExplorip
{
    public TabItemFileSystemManagement(string path)
    {
        InitializeComponent();
        DataContext = new TabItemFileSystemManagementViewModel(path);
    }

    public new TabItemFileSystemManagementViewModel DataContext
    {
        get { return (TabItemFileSystemManagementViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }
}
