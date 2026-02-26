using System;
using System.Windows;

namespace Mp3Player;

/// <summary>
/// Logique d'interaction pour MyMp3PlayerPlayListWindow.xaml
/// </summary>
public partial class MyMp3PlayerPlayListWindow : Window
{
    public MyMp3PlayerPlayListWindow()
    {
        InitializeComponent();
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        Hide();
    }
}
