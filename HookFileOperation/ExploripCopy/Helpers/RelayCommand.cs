using System;
using System.Windows.Input;

namespace ExploripCopy.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> m_execute;
        private readonly Predicate<object> m_canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
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
}
