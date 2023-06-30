using System;
using System.Windows.Input;

using ExploripCopy.GUI;
using ExploripCopy.Helpers;

using Hardcodet.Wpf.TaskbarNotification;

namespace ExploripCopy.ViewModels
{
    internal class NotifyIconViewModel : ViewModelBase
    {
        public static NotifyIconViewModel Instance { get; private set; }

        private TaskbarIcon _notifyIcon;
        public ICommand ExitApplicationCommand { get; private set; }
        public ICommand ShowWindowCommand { get; private set; }

        public NotifyIconViewModel() : base()
        {
            Instance = this;
            SetSystrayIcon(false);
            ExitApplicationCommand = new RelayCommand(new Action<object>((param) => Exit()));
            ShowWindowCommand = new RelayCommand(new Action<object>((param) => Show()));
        }

        private void Exit()
        {
            MainWindow.Instance.IconInSystray_Exit();
        }

        private void Show()
        {
            MainWindow.Instance.ShowWindow();
        }

        public void SetSystrayIcon(bool working)
        {
            SystrayIcon = (working ? "/Resources/SystrayIconR.ico" : "/Resources/SystrayIconG.ico");
        }

        private string _currentIcon;
        public string SystrayIcon
        {
            get { return _currentIcon; }
            set
            {
                if (_currentIcon != value)
                {
                    _currentIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        public TaskbarIcon SystrayControl
        {
            get { return _notifyIcon; }
        }

        public void SetControl(TaskbarIcon myControl)
        {
            _notifyIcon = myControl;
        }
    }
}
