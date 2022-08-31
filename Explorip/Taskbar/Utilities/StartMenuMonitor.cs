using System;
using System.Windows.Threading;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Common.SupportingClasses;
using ManagedShell.Interop;

namespace Explorip.TaskBar.Utilities
{
    public class StartMenuMonitor : IDisposable
    {
        private readonly AppVisibilityHelper _appVisibilityHelper;
        private DispatcherTimer _poller;
        private bool _isVisible;

        public event EventHandler<LauncherVisibilityEventArgs> StartMenuVisibilityChanged;

        public StartMenuMonitor(AppVisibilityHelper appVisibilityHelper)
        {
            _appVisibilityHelper = appVisibilityHelper;
            SetupPoller();
        }

        private void SetupPoller()
        {
            if (_poller == null)
            {
                _poller = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };

                _poller.Tick += Poller_Tick;
            }

            _poller.Start();
        }

        private void Poller_Tick(object sender, EventArgs e)
        {
            if (EnvironmentHelper.IsWindows8OrBetter)
            {
                // Windows 8+
                SetVisibility(IsModernStartMenuOpen());
            }

            if (!EnvironmentHelper.IsWindows8OrBetter || !_isVisible)
            {
                // Windows 7, StartIsBack, Start8+
                SetVisibility(IsClassicStartMenuOpen());
            }

            if (!_isVisible)
            {
                // Open Shell Menu
                SetVisibility(IsOpenShellMenuOpen());
            }
        }

        private void SetVisibility(bool isVisible)
        {
            if (isVisible == _isVisible)
            {
                return;
            }

            _isVisible = isVisible;

            LauncherVisibilityEventArgs args = new()
            {
                Visible = _isVisible
            };

            StartMenuVisibilityChanged?.Invoke(this, args);
        }

        private bool IsModernStartMenuOpen()
        {
            if (_appVisibilityHelper == null)
            {
                ShellLogger.Error("StartMenuMonitor: AppVisibilityHelper is null");
                return false;
            }

            return _appVisibilityHelper.IsLauncherVisible();
        }

        private bool IsClassicStartMenuOpen()
        {
            return IsVisibleByClass("DV2ControlHost");
        }

        private bool IsOpenShellMenuOpen()
        {
            return IsVisibleByClass("OpenShell.CMenuContainer");
        }

        private bool IsVisibleByClass(string className)
        {
            IntPtr hStartMenu = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, className, IntPtr.Zero);

            return hStartMenu != IntPtr.Zero && NativeMethods.IsWindowVisible(hStartMenu);
        }

        public void Dispose()
        {
            _poller?.Stop();
        }
    }
}
