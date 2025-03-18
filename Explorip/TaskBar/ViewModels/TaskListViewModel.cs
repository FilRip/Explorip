using System;
using System.ComponentModel;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskListViewModel : ObservableObject, IDisposable
{
    private AppBarEdge _currentEdge;

    [ObservableProperty()]
    private ICollectionView _taskListCollection;
    private bool disposedValue;

    public TaskListViewModel() : base()
    {

    }

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(PanelOrientation));
    }

    public Taskbar TaskbarParent { get; set; }

    public Orientation PanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
