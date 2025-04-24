using Explorip.TaskBar.Controls;

namespace Explorip.TaskBar.ViewModels;

public partial class ToolbarPluginViewModel : BaseToolbarViewModel
{
    private string _guid;

    public override string Id => _guid;

    public override void Init(BaseToolbar parentControl)
    {
        base.Init(parentControl);
        _guid = ((ToolbarPlugin)parentControl).PluginLinked.GuidKey.ToString();
        DefaultSavedPosition();
    }
}
