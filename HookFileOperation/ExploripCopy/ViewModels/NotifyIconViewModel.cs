using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExploripCopy.GUI;

using Hardcodet.Wpf.TaskbarNotification;

namespace ExploripCopy.ViewModels
{
    internal partial class NotifyIconViewModel : ObservableObject
    {
        public static NotifyIconViewModel Instance { get; private set; }

        private TaskbarIcon _notifyIcon;

        public NotifyIconViewModel() : base()
        {
            Instance = this;
            SetSystrayIcon(false);
        }

        [RelayCommand()]
        private void ExitApplication()
        {
            MainWindow.Instance.IconInSystray_Exit();
        }

        [RelayCommand()]
        private void ShowWindow()
        {
            MainWindow.Instance.ShowWindow();
        }

        public void SetSystrayIcon(bool working)
        {
            SystrayIcon = (working ? "/Resources/SystrayIconR.ico" : "/Resources/SystrayIconG.ico");
        }

        [ObservableProperty()]
        private string _systrayIcon;

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
