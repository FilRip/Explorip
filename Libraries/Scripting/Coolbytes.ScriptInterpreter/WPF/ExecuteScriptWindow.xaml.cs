using System.Windows;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class ExecuteScriptWindow : Window
{
    public ExecuteScriptWindow()
    {
        InitializeComponent();
    }

    public WpfInterpreteScript ComposantInterpreteScript
    {
        get { return InterpreteScript; }
    }
}
