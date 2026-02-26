using CoolBytes.ScriptInterpreter.WPF.ViewModels;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class WpfObject
{
    public WpfObject()
    {
        InitializeComponent();
    }

    public new WpfObjectViewModel DataContext
    {
        get { return (WpfObjectViewModel)base.DataContext; }
    }

    public void LoadXml(string contenuXml)
    {
        DataContext.LoadXml(contenuXml);
    }

    public void LoadFile(string nomFichier)
    {
        DataContext.LoadFile(nomFichier);
    }
}
