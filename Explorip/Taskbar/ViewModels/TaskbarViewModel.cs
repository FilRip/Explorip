using System.Windows;

using Explorip.Helpers;

namespace Explorip.TaskBar.ViewModels
{
    public class TaskbarViewModel : ViewModelBase
    {
        private static TaskbarViewModel _instance;

        public static TaskbarViewModel Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        private TaskbarViewModel() : base()
        {
            ShowTabTip = Visibility.Hidden;
        }

        private bool _resizeMode;
        public bool ResizeOn
        {
            get { return _resizeMode; }
            set
            {
                _resizeMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LabelUnlock));
            }
        }
        public string LabelUnlock
        {
            get
            {
                if (ResizeOn)
                    return "Lock";
                else
                    return "Unlock";
            }
        }

        private Visibility _showKeyboard;
        public Visibility ShowTabTip
        {
            get { return _showKeyboard; }
            set
            {
                _showKeyboard = value;
                OnPropertyChanged();
            }
        }
    }
}
