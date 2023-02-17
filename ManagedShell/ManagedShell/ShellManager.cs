using System;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.WindowsTasks;
using ManagedShell.WindowsTray;

namespace ManagedShell
{
    public class ShellManager : IDisposable
    {
        public static readonly ShellConfig DefaultShellConfig = new()
        {
            EnableTasksService = true,
            AutoStartTasksService = true,
            TaskIconSize = TasksService.DEFAULT_ICON_SIZE,

            EnableTrayService = true,
            AutoStartTrayService = true,
            PinnedNotifyIcons = NotificationArea.DEFAULT_PINNED
        };
        private bool disposedValue;

        /// <summary>
        /// Initializes ManagedShell with the default configuration.
        /// </summary>
        public ShellManager() : this(DefaultShellConfig)
        {
        }

        /// <summary>
        /// Initializes ManagedShell with a custom configuration.
        /// </summary>
        /// <param name="config">A ShellConfig struct containing desired initialization parameters.</param>
        public ShellManager(ShellConfig config)
        {
            if (config.EnableTrayService)
            {
                TrayService = new TrayService();
                ExplorerTrayService = new ExplorerTrayService();
                NotificationArea = new NotificationArea(config.PinnedNotifyIcons, TrayService, ExplorerTrayService);
            }

            if (config.EnableTasksService)
            {
                TasksService = new TasksService(config.TaskIconSize);
                Tasks = new Tasks(TasksService);
            }

            FullScreenHelper = new FullScreenHelper();
            ExplorerHelper = new ExplorerHelper(NotificationArea);
            AppBarManager = new AppBarManager(ExplorerHelper);

            if (config.EnableTrayService && config.AutoStartTrayService)
            {
                NotificationArea.Initialize();
            }

            if (config.EnableTasksService && config.AutoStartTasksService)
            {
                Tasks.Initialize();
            }
        }

        public NotificationArea NotificationArea { get; }
        public TrayService TrayService { get; }
        public ExplorerTrayService ExplorerTrayService { get; }

        public TasksService TasksService { get; }
        public Tasks Tasks { get; }

        public AppBarManager AppBarManager { get; }
        public ExplorerHelper ExplorerHelper { get; }
        public FullScreenHelper FullScreenHelper { get; }

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
                    IconHelper.DisposeIml();

                    AppBarManager.Dispose();
                    FullScreenHelper.Dispose();
                    NotificationArea?.Dispose();
                    Tasks?.Dispose();
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
}