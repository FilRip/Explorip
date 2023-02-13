using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Threading;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.WindowsTray;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.AppBar
{
    public class ExplorerHelper
    {
        private static TaskbarState? startupTaskbarState;
        private readonly NotificationArea _notificationArea;


        private readonly DispatcherTimer taskbarMonitor = new(DispatcherPriority.Background);

        private bool _hideExplorerTaskbar;

        public bool HideExplorerTaskbar
        {
            get => _hideExplorerTaskbar;

            set
            {
                if (value != _hideExplorerTaskbar)
                {
                    _hideExplorerTaskbar = value;

                    if (_hideExplorerTaskbar)
                    {
                        HideTaskbar();
                    }
                    else
                    {
                        ShowTaskbar();
                    }
                }
            }
        }

        public ExplorerHelper() : this(null)
        {
        }

        public ExplorerHelper(NotificationArea notificationArea)
        {
            _notificationArea = notificationArea;

            SetupTaskbarMonitor();
        }

        public void SuspendTrayService()
        {
            // get Explorer tray window back so we can do appbar stuff that requires shared memory
            _notificationArea?.Suspend();
        }

        public void ResumeTrayService()
        {
            // take back over
            _notificationArea?.Resume();
        }

        public void SetTaskbarVisibility(int swp)
        {
            // only run this if our TaskBar is enabled, or if we are showing the Windows TaskBar
            if (swp != (int)SWP.SWP_HIDEWINDOW || HideExplorerTaskbar)
            {
                IntPtr taskbarHwnd = WindowHelper.FindWindowsTray(_notificationArea.Handle);
                IntPtr startButtonHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);

                if (taskbarHwnd != IntPtr.Zero
                    && swp == (int)SWP.SWP_HIDEWINDOW == IsWindowVisible(taskbarHwnd))
                {
                    SetWindowPos(taskbarHwnd, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, swp | (int)SWP.SWP_NOMOVE | (int)SWP.SWP_NOSIZE | (int)SWP.SWP_NOACTIVATE);
                    if (startButtonHwnd != IntPtr.Zero)
                    {
                        SetWindowPos(startButtonHwnd, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, swp | (int)SWP.SWP_NOMOVE | (int)SWP.SWP_NOSIZE | (int)SWP.SWP_NOACTIVATE);
                    }
                }

                // adjust secondary TaskBars for multi-monitor
                SetSecondaryTaskbarVisibility(swp);
            }
        }

        public void SetSecondaryTaskbarVisibility(int swp)
        {
            IntPtr secTaskbarHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_SecondaryTrayWnd", null);

            // if we have 3+ monitors there may be multiple secondary TaskBars
            while (secTaskbarHwnd != IntPtr.Zero)
            {
                if (swp == (int)SWP.SWP_HIDEWINDOW == IsWindowVisible(secTaskbarHwnd))
                {
                    SetWindowPos(secTaskbarHwnd, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, swp | (int)SWP.SWP_NOMOVE | (int)SWP.SWP_NOSIZE | (int)SWP.SWP_NOACTIVATE);
                }

                secTaskbarHwnd = FindWindowEx(IntPtr.Zero, secTaskbarHwnd, "Shell_SecondaryTrayWnd", null);
            }
        }

        public void SetTaskbarState(TaskbarState state)
        {
            Task.Run(() =>
            {
                AppBarData abd = new()
                {
                    cbSize = Marshal.SizeOf(typeof(AppBarData)),
                    hWnd = WindowHelper.FindWindowsTray(_notificationArea.Handle),
                    lParam = (IntPtr)state
                };

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);
            });
        }

        public TaskbarState GetTaskbarState()
        {
            AppBarData abd = new()
            {
                cbSize = Marshal.SizeOf(typeof(AppBarData)),
                hWnd = WindowHelper.FindWindowsTray(_notificationArea.Handle)
            };

            uint uState = SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd);

            return (TaskbarState)uState;
        }

        private void HideTaskbar()
        {
            if (!EnvironmentHelper.IsAppRunningAsShell)
            {
                if (startupTaskbarState == null)
                {
                    startupTaskbarState = GetTaskbarState();
                }

                if (HideExplorerTaskbar)
                {
                    DoHideTaskbar();
                    taskbarMonitor.Start();
                }
            }
        }

        private void DoHideTaskbar()
        {
            SetTaskbarState(TaskbarState.AutoHide);
            SetTaskbarVisibility((int)SWP.SWP_HIDEWINDOW);
        }

        private void ShowTaskbar()
        {
            if (!EnvironmentHelper.IsAppRunningAsShell)
            {
                SetTaskbarState(startupTaskbarState ?? TaskbarState.OnTop);
                SetTaskbarVisibility((int)SWP.SWP_SHOWWINDOW);
                taskbarMonitor.Stop();
            }
        }

        private void SetupTaskbarMonitor()
        {
            taskbarMonitor.Interval = new TimeSpan(0, 0, 0, 0, 100);
            taskbarMonitor.Tick += TaskbarMonitor_Tick;
        }

        private void TaskbarMonitor_Tick(object sender, EventArgs e)
        {
            IntPtr taskbarHwnd = WindowHelper.FindWindowsTray(_notificationArea.Handle);

            if (IsWindowVisible(taskbarHwnd))
            {
                ShellLogger.Debug("ExplorerHelper: Hiding unwanted Windows taskbar");
                DoHideTaskbar();
                return;
            }

            IntPtr secTaskbarHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_SecondaryTrayWnd", null);

            // if we have 3+ monitors there may be multiple secondary TaskBars
            while (secTaskbarHwnd != IntPtr.Zero)
            {
                if (IsWindowVisible(secTaskbarHwnd))
                {
                    ShellLogger.Debug("ExplorerHelper: Hiding unwanted Windows taskbar");
                    DoHideTaskbar();
                    return;
                }

                secTaskbarHwnd = FindWindowEx(IntPtr.Zero, secTaskbarHwnd, "Shell_SecondaryTrayWnd", null);
            }
        }



        public enum TaskbarState
        {
            AutoHide = 1,
            OnTop = 0
        }
    }
}