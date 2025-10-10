using System;
using System.Windows.Input;

namespace CoolBytes.ScriptInterpreter.Helpers;

internal class RelayCommand : ICommand
{
    internal readonly Action<object> m_execute;
    internal readonly Predicate<object> m_canExecute;

    internal RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        m_execute = execute ?? throw new ArgumentNullException(nameof(execute));

        m_canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return m_canExecute == null || m_canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        m_execute(parameter ?? "<N/A>");
    }
}
