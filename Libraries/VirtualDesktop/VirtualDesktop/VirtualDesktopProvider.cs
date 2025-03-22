using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsDesktop.Interop;

namespace WindowsDesktop
{
    public class VirtualDesktopProvider : IDisposable
    {
        #region Default instance

        private static readonly Lazy<VirtualDesktopProvider> _default = new(() => new VirtualDesktopProvider());

        public static VirtualDesktopProvider Default => _default.Value;

        #endregion

        private Task _initializationTask;
        private Task _initializationTerminatedTask;
        private ComObjects _comObjects;
        private bool disposedValue;

        public string ComInterfaceAssemblyPath { get; set; }

        public bool AutoRestart { get; set; } = true;

        internal ComObjects ComObjects
        {
            get
            {
                while (!_comObjects.IsAvailable)
                {
                    Thread.Sleep(1);
                }
                return _comObjects;
            }
            private set => _comObjects = value;
        }

        public Task Initialize()
            => Initialize(TaskScheduler.FromCurrentSynchronizationContext());

        public Task Initialize(TaskScheduler scheduler)
        {
            if (_initializationTask == null)
            {
                _initializationTask = Task.Run(() => Core());
                _initializationTerminatedTask = Task.CompletedTask;
                if (AutoRestart && scheduler != null)
                {
                    _initializationTerminatedTask = _initializationTask.ContinueWith(ContinueTask,
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        scheduler);
                }
            }

            return _initializationTask;

            void Core()
            {
                ComInterfaceAssemblyProvider assemblyProvider = new(ComInterfaceAssemblyPath);
                ComInterfaceAssembly assembly = new(assemblyProvider.GetAssembly());

                ComObjects = new ComObjects(assembly);
            }
        }

        public Task TerminatedInitializedTask
        {
            get { return _initializationTerminatedTask; }
        }

        private void ContinueTask(Task task)
        {
            try
            {
                if (task.IsCompleted)
                {
                    ComObjects.Listen();
                    Initialized = true;
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }

        public bool Initialized { get; private set; }

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
                    ComObjects?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    partial class VirtualDesktop
    {
        public static VirtualDesktopProvider Provider { get; set; }

        internal static VirtualDesktopProvider ProviderInternal
            => Provider ?? VirtualDesktopProvider.Default;
    }
}
