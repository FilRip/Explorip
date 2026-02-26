using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemWindowEmbedded.xaml
/// </summary>
public partial class TabItemWindowEmbedded : TabItemExplorip
{
    public TabItemWindowEmbedded()
    {
        InitializeComponent();
        InitializeExplorip();
        OnSelecting += TabItemWindowEmbedded_OnSelecting;
        OnDeSelecting += TabItemWindowEmbedded_OnDeSelecting;
        base.DataContext = new TabItemWindowEmbeddedViewModel(this);
    }

    public new TabItemWindowEmbeddedViewModel DataContext
    {
        get { return (TabItemWindowEmbeddedViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void TabItemWindowEmbedded_OnDeSelecting()
    {
        EmbeddedWindow.Hide();
    }

    private void TabItemWindowEmbedded_OnSelecting()
    {
        EmbeddedWindow.Show();
    }

    private void TabItemExplorip_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        SetTitle("Embedded window");
    }

    public void Reset()
    {
        DataContext.Enabled = false;
    }
}
