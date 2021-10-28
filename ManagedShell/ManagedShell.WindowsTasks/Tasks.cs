using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ManagedShell.WindowsTasks
{
    public class Tasks : IDisposable
    {
        private readonly TasksService _tasksService;
        private readonly ICollectionView groupedWindows;

        public ICollectionView GroupedWindows => groupedWindows;

        public Tasks(TasksService tasksService)
        {
            _tasksService = tasksService;
            // prepare collections
            groupedWindows = CollectionViewSource.GetDefaultView(_tasksService.Windows);
            groupedWindows.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            groupedWindows.CollectionChanged += GroupedWindows_Changed;
            groupedWindows.Filter = GroupedWindows_Filter;

            if (groupedWindows is ICollectionViewLiveShaping taskbarItemsView)
            {
                taskbarItemsView.IsLiveFiltering = true;
                taskbarItemsView.LiveFilteringProperties.Add("ShowInTaskbar");
                taskbarItemsView.IsLiveGrouping = true;
                taskbarItemsView.LiveGroupingProperties.Add("Category");
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
            _tasksService.Initialize();
        }

        private void GroupedWindows_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            // yup, do nothing. helps prevent a NRE
        }

        private bool GroupedWindows_Filter(object item)
        {
            if (item is ApplicationWindow window && window.ShowInTaskbar)
                return true;

            return false;
        }

        public void Dispose()
        {
            _tasksService.Dispose();
        }
    }
}
