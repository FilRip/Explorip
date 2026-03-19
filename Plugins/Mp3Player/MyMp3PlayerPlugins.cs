using System;
using System.Windows.Controls;
using System.Windows.Media;

using ExploripPlugins;

using ManagedShell.AppBar;

using Mp3Player.Controls;

namespace Mp3Player;

public class MyMp3PlayerPlugins : IExploripToolbar
{
    private MyMp3PlayerControl _userControl;

    public string Author { get => "FilRip"; }

    public string Name { get => "MP3 Player"; }

    public string Description { get => "This is a simple MP3 player for plugins demo"; }

    public Version Version { get => new(1, 0, 0, 0); }

    public UserControl ExploripToolbar
    {
        get
        {
            _userControl ??= new MyMp3PlayerControl();
            return _userControl;
        }
    }

    public double MinHeight { get => 24; }

    public double MinWidth { get => 0; }

    public Guid GuidKey { get => new("{921DE993-35F2-4B88-9832-F7F837929CDC}"); }

    public void DisableDisplay()
    {
        _userControl.DataContext.DisableDisplay();
    }

    public void EnableDisplay()
    {
        _userControl.DataContext.EnableDisplay();
    }

    public void SetGlobalColors(SolidColorBrush background, SolidColorBrush foreground, SolidColorBrush accent)
    {
        _userControl.DataContext.SetColor(background, foreground);
    }

    public void UpdateTaskbar(int numScreen, double width, double height, Brush backgroundColor, AppBarEdge edge)
    {
        _userControl.DataContext.ChangeTaskbarBackgroundColor(backgroundColor);
    }
}
