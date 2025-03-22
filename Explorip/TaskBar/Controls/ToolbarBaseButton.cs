using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Constants;
using Explorip.Helpers;

using ManagedShell.ShellFolders;

namespace Explorip.TaskBar.Controls;

public class ToolbarBaseButton : UserControl
{
    private bool _startDrag;

    public ToolbarBaseButton() : base()
    {
        PreviewMouseDown += DragMouseDown;
        PreviewMouseUp += DragMouseUp;
        DragEnter += OnDragEnter;
    }

    protected void DragMouseDown(object sender, MouseButtonEventArgs e)
    {
        _startDrag = true;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        StartDrag();
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
    }

    protected void DragMouseUp(object sender, MouseButtonEventArgs e)
    {
        _startDrag = false;
    }

    public ShellFile MyDataContext
    {
        get { return (ShellFile)DataContext; }
    }

    protected void OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data)
        {
            ShellFile shellFile = (ShellFile)data.GetData(typeof(ShellFile));
            if (shellFile != DataContext)
            {
                (MyDataContext.Position, shellFile.Position) = (shellFile.Position, MyDataContext.Position);
                this.FindVisualParent<Toolbar>().MyDataContext.RefreshMyCollectionView();
            }
        }
    }

    private async Task StartDrag()
    {
        await Task.Delay(WindowsConstants.DelayIgnoreDrag);
        if (_startDrag)
        {
            DataObject data = new();
            data.SetData(MyDataContext);
            DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            _startDrag = true;
        }
    }
}
