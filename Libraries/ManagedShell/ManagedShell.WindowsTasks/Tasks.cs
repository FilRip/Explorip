using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ManagedShell.WindowsTasks;

public class Tasks : IDisposable
{
    private readonly TasksService _tasksService;

    public ICollectionView GroupedWindows { get; set; }
    public bool SetInitialWindows { get; set; } = true;

    public Tasks(TasksService tasksService)
    {
        _tasksService = tasksService;
        // prepare collections
        GroupedWindows = CollectionViewSource.GetDefaultView(_tasksService.Windows);
        GroupedWindows.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ApplicationWindow.Category)));
        GroupedWindows.CollectionChanged += GroupedWindows_Changed;
        GroupedWindows.Filter = GroupedWindows_Filter;

        if (GroupedWindows is ICollectionViewLiveShaping taskbarItemsView)
        {
            taskbarItemsView.IsLiveFiltering = true;
            taskbarItemsView.LiveFilteringProperties.Add(nameof(ApplicationWindow.ShowInTaskbar));
            taskbarItemsView.IsLiveGrouping = true;
            taskbarItemsView.LiveGroupingProperties.Add(nameof(ApplicationWindow.Category));
        }
    }

    public void Initialize(ITaskCategoryProvider taskCategoryProvider)
    {
        if (!_tasksService.IsInitialized)
        {
            _tasksService.SetTaskCategoryProvider(taskCategoryProvider);
            Initialize();
        }
    }

    public void Initialize()
    {
        _tasksService.Initialize(SetInitialWindows);
    }

    private void GroupedWindows_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
        // yup, do nothing. helps prevent a NRE
    }

    private static bool GroupedWindows_Filter(object item)
    {
        if (item is ApplicationWindow window && window.ShowInTaskbar)
            return true;

        return false;
    }

    private bool _isDisposed;
    public bool IsDisposed
    {
        get { return _isDisposed; }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _tasksService.Dispose();
            }
            _isDisposed = true;
        }
    }
}
