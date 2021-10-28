using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop.Interop;

namespace WindowsDesktop
{
    public class VirtualDesktopProvider : IDisposable
    {
        #region Default instance

        private static readonly Lazy<VirtualDesktopProvider> _default = new Lazy<VirtualDesktopProvider>(() => new VirtualDesktopProvider());

        public static VirtualDesktopProvider Default => _default.Value;

        #endregion

        private Task _initializationTask;
        private ComObjects _comObjects;

        public string ComInterfaceAssemblyPath { get; set; }

        public bool AutoRestart { get; set; } = true;

        internal ComObjects ComObjects {
            get { 
                while(!_comObjects.IsAvailable) {
                    Thread.Sleep(1);
                }
                return _comObjects; 
            }
            private set => _comObjects = value;
        }

        public Task Initialize()
            => this.Initialize(TaskScheduler.FromCurrentSynchronizationContext());

        public Task Initialize(TaskScheduler scheduler)
        {
            if (this._initializationTask == null)
            {
                this._initializationTask = Task.Run(() => Core());

                if (this.AutoRestart && scheduler != null)
                {
                    this._initializationTask.ContinueWith(
                        _ => this.ComObjects.Listen(),
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        scheduler);
                }
            }

            return this._initializationTask;

            void Core()
            {
                var assemblyProvider = new ComInterfaceAssemblyProvider(this.ComInterfaceAssemblyPath);
                var assembly = new ComInterfaceAssembly(assemblyProvider.GetAssembly());

                this.ComObjects = new ComObjects(assembly);
            }
        }

        public void Dispose()
        {
            this.ComObjects?.Dispose();
        }
    }

    partial class VirtualDesktop
    {
        public static VirtualDesktopProvider Provider { get; set; }

        internal static VirtualDesktopProvider ProviderInternal
            => Provider ?? VirtualDesktopProvider.Default;
    }
}
