using System;

using WindowsDesktop.Internal;
using WindowsDesktop.Properties;

namespace WindowsDesktop.Interop
{
    internal class ComObjects : IDisposable
    {
        private readonly ComInterfaceAssembly _assembly;
        private ExplorerRestartListenerWindow _listenerWindow;
        private IDisposable _listener;
        private bool disposedValue;

        public IVirtualDesktopManager VirtualDesktopManager { get; private set; }

        public VirtualDesktopManagerInternal VirtualDesktopManagerInternal { get; private set; }

        public VirtualDesktopNotificationService VirtualDesktopNotificationService { get; private set; }

        public VirtualDesktopPinnedApps VirtualDesktopPinnedApps { get; private set; }

        public ApplicationViewCollection ApplicationViewCollection { get; private set; }

        public bool IsAvailable { get; private set; } = false;

        public ComObjects(ComInterfaceAssembly assembly)
        {
            _assembly = assembly;
            Initialize();
        }

        public void Listen()
        {
            _listenerWindow = new ExplorerRestartListenerWindow(() => Initialize());
            _listenerWindow.Show();
        }

        private void Initialize()
        {
            IsAvailable = false;
            VirtualDesktopCache.Initialize(_assembly);

            VirtualDesktopManager = (IVirtualDesktopManager)Activator.CreateInstance(Type.GetTypeFromCLSID(ClSid.VirtualDesktopManager));
            if (ProductInfo.OSBuild >= 22631)
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal22631(_assembly);
            }
            else if (ProductInfo.OSBuild >= 22449)
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal22449(_assembly);
            }
            else if (ProductInfo.OSBuild >= 21359)
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal21359(_assembly);
            }
            else if (ProductInfo.OSBuild >= 21313)
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal21313(_assembly);
            }
            else if (ProductInfo.OSBuild >= 20231)
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal20231(_assembly);
            }
            else
            {
                VirtualDesktopManagerInternal = new VirtualDesktopManagerInternal10240(_assembly);
            }
            VirtualDesktopNotificationService = new VirtualDesktopNotificationService(_assembly);
            VirtualDesktopPinnedApps = new VirtualDesktopPinnedApps(_assembly);
            ApplicationViewCollection = new ApplicationViewCollection(_assembly);

            _listener?.Dispose();
            _listener = VirtualDesktopNotificationService.Register(VirtualDesktopNotification.CreateInstance(_assembly));
            IsAvailable = true;
        }

        private sealed class ExplorerRestartListenerWindow : TransparentWindow
        {
            private uint _explorerRestartedMessage;
            private readonly Action _action;

            public ExplorerRestartListenerWindow(Action action)
            {
                Name = nameof(ExplorerRestartListenerWindow);
                _action = action;
            }

            public override void Show()
            {
                base.Show();
                _explorerRestartedMessage = NativeMethods.RegisterWindowMessage("TaskbarCreated");
            }

            protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == _explorerRestartedMessage)
                {
                    _action();
                    return IntPtr.Zero;
                }

                return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
            }
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
                    _listener?.Dispose();
                    _listenerWindow?.Close();
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
