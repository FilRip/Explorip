using System;
using System.Windows.Controls;

using ExploripPlugins;

namespace Mp3Player;

public class MyMp3PlayerPlugins : IExploripToolbar
{
    private MyMp3PlayerControl _userControl;

    public string Author { get => "FilRip"; }

    public string Name { get => "MP3 Player"; }

    public string Description { get => "This is a simple MP3 player for plugins demo"; }

    public Version Version { get => new(1,0,0,0); }

    public UserControl ExploripToolbar
    {
        get
        {
            _userControl ??= new MyMp3PlayerControl();
            return _userControl;
        }
    }

    public double MinHeight { get => 16; }

    public double MinWidth { get => 0; }

    public void SpecifyTaskbarSize(string screenName, int width, int height)
    {
        // Nothing to do here, or not yet to be precise
    }
}
