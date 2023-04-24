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

        private TaskbarViewModel() : base() { }

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
    }
}
