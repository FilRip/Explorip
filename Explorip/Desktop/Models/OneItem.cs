using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Desktop.Models
{
    internal partial class OneItem : ObservableObject
    {
        [ObservableProperty()]
        private string _name;

        [ObservableProperty()]
        private ImageSource _icon;
    }
}
