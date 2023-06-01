using System;
using System.Windows.Input;

using ExploripCopy.GUI;
using ExploripCopy.Helpers;

namespace ExploripCopy.ViewModels
{
    internal class NotifyIconViewModel : ViewModelBase
    {
        public ICommand ExitApplicationCommand { get; private set; }
        public ICommand ShowWindowCommand { get; private set; }

        public NotifyIconViewModel() : base()
        {
            ExitApplicationCommand = new RelayCommand(new Action<object>((param) => Exit()));
            ShowWindowCommand = new RelayCommand(new Action<object>((param) => Show()));
        }

        private void Exit()
        {
            MainWindow.Instance.IconInSystray_Exit();
        }

        private void Show()
        {
            MainWindow.Instance.IconInSystray_DoubleClick();
        }
    }
}
