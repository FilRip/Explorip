using System.Windows.Controls;

namespace Mp3Player;

/// <summary>
/// Logique d'interaction pour MyMp3PlayerControl.xaml
/// </summary>
public partial class MyMp3PlayerControl : UserControl
{
    public MyMp3PlayerControl()
    {
        InitializeComponent();
    }

    public new MyMp3PlayerPluginViewModel DataContext
    {
        get { return (MyMp3PlayerPluginViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }
}
