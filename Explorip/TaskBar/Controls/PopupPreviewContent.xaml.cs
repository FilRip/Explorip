using System.Windows.Controls.Primitives;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

public partial class PopupPreviewContent : Popup
{
    public PopupPreviewContent()
    {
        InitializeComponent();
    }

    public new PopupPreviewContentViewModel DataContext
    {
        get { return (PopupPreviewContentViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }
}
