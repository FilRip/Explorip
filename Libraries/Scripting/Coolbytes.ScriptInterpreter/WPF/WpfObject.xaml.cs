using System;
using System.Windows.Controls;

using CoolBytes.ScriptInterpreter.WPF.ViewModels;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class WpfObject
{
    public WpfObject()
    {
        InitializeComponent();
    }

    public void LoadXml(string contenuXml)
    {
        ((WpfObjectViewModel)DataContext).LoadXml(contenuXml);
    }

    public void LoadFile(string nomFichier)
    {
        ((WpfObjectViewModel)DataContext).LoadFile(nomFichier);
    }

    private void TvObjet_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        try
        {
            ((WpfObjectViewModel)DataContext).SelectionTree = (TreeViewItem)e.NewValue;
        }
        catch (Exception) { /* Ignore errors */ }
    }
}
