using System.ComponentModel;
using System.Runtime.CompilerServices;

using ManagedShell;

namespace Explorip.TaskBar.ViewModels
{
    public class TaskbarViewModel : ShellManager, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}
