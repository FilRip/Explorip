using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ManagedShell.WindowsTasks
{
    public class Tasks : IDisposable
    {
        private readonly TasksService _tasksService;

        public ICollectionView GroupedWindows { get; set; }

        public Tasks(TasksService tasksService)
        {
            _tasksService = tasksService;
            // prepare collections
            GroupedWindows = CollectionViewSource.GetDefaultView(_tasksService.Windows);
            GroupedWindows.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            GroupedWindows.CollectionChanged += GroupedWindows_Changed;
            GroupedWindows.Filter = GroupedWindows_Filter;

            if (GroupedWindows is ICollectionViewLiveShaping taskbarItemsView)
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
