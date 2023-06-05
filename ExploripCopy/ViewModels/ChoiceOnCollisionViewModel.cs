using ExploripCopy.Models;

namespace ExploripCopy.ViewModels
{
    public class ChoiceOnCollisionViewModel : ViewModelBase
    {
        public EChoiceFileOperation Choice { get; set; } = EChoiceFileOperation.KeepMostRecent;
    }
}
