using CommunityToolkit.Mvvm.ComponentModel;

using ExploripCopy.Models;

namespace ExploripCopy.ViewModels
{
    public partial class ChoiceOnCollisionViewModel : ObservableObject
    {
        public ChoiceOnCollisionViewModel() : base()
        {
            Choice = EChoiceFileOperation.KeepMostRecent;
            DoSameForAllFiles = true;
        }

        [ObservableProperty()]
        private EChoiceFileOperation _choice;

        [ObservableProperty()]
        private bool _doSameForAllFiles;

        private string _conflictFile;
        public string ConflictFile
        {
            get
            {
                return _conflictFile;
            }
            set
            {
                _conflictFile = Constants.Localization.FILE_NAME_EXIST.Replace("%s", value);
                OnPropertyChanged();
            }
        }
    }
}
