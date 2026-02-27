using System.Windows;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class ExecuteScriptWindow : Window
{
    public ExecuteScriptWindow()
    {
        InitializeComponent();
    }

    public WpfInterpreteScript InterpreteScriptComponent
    {
        get { return InterpreteScript; }
    }
}
