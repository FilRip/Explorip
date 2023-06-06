using ExploripCopy.Models;

namespace ExploripCopy.ViewModels
{
    public class ChoiceOnCollisionViewModel : ViewModelBase
    {
        public ChoiceOnCollisionViewModel() : base()
        {
            Choice = EChoiceFileOperation.KeepMostRecent;
            DoSameForAllFiles = true;
        }

        private EChoiceFileOperation _choice;
        public EChoiceFileOperation Choice
        {
            get { return _choice; }
            set
            {
                _choice = value;
                OnPropertyChanged();
            }
        }

        private bool _sameForAllFiles;
        public bool DoSameForAllFiles
        {
            get { return _sameForAllFiles; }
            set
            {
                _sameForAllFiles = value;
                OnPropertyChanged();
            }
        }

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
