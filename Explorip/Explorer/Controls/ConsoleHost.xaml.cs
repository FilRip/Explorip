﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

using Explorip.Helpers;
using Explorip.WinAPI;

using ManagedShell.Interop;

namespace Explorip.Explorer.Controls
{
    /// <summary>
    /// Logique d'interaction pour ConsoleHost.xaml
    /// </summary>
    public partial class ConsoleHost : UserControl, IDisposable
    {
        private const int OFFSET_X = 6;
        private const int OFFSET_Y = 62;
        private const int OFFSET_SIZE_HEIGHT = 24;

        private Process _myProcess;
        private IntPtr _srcPtr, _destPtr;

        public ConsoleHost()
        {
            InitializeComponent();
            _destPtr = IntPtr.Zero;
        }

        private bool InitProcess(ProcessStartInfo psi)
        {
            _myProcess = new Process()
            {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            _myProcess.Exited += MyProcess_Exited;
            try
            {
                _myProcess.Start();
                while (_myProcess.MainWindowHandle == IntPtr.Zero) { /* Nothing to do, waiting process start */ }
                _srcPtr = _myProcess.MainWindowHandle;
                return true;
            }
            catch (Exception) { /* Ignore errors */ }
            return false;
        }

        public bool StartProcess(string commandline)
        {
            if (_srcPtr != IntPtr.Zero)
                return true;
            _destPtr = (PresentationSource.FromVisual(this) as HwndSource).Handle;
            if (InitProcess(new ProcessStartInfo() { FileName = commandline }))
            {
                InitRedirectWindow();
                return true;
            }
            return false;
        }

        public bool StartProcess(ProcessStartInfo psi)
        {
            if (_srcPtr != IntPtr.Zero)
                return true;
            _destPtr = (PresentationSource.FromVisual(this) as HwndSource).Handle;
            if (InitProcess(psi))
            {
                InitRedirectWindow();
                return true;
            }
            return false;
        }

        private void InitRedirectWindow()
        {
            User32.SetParent(_srcPtr, _destPtr);
            User32.SetWindowPos(_srcPtr, IntPtr.Zero, OFFSET_X + (int)this.FindVisualParent<TabExplorerBrowser>().GetVisualOffset().X, OFFSET_Y, (int)ActualWidth - OFFSET_X, (int)ActualHeight - OFFSET_SIZE_HEIGHT, User32.SWP.SHOWWINDOW);
            int currentStyle = NativeMethods.GetWindowLong(_srcPtr, NativeMethods.GWL_STYLE);
            NativeMethods.SetWindowLong(_srcPtr, NativeMethods.GWL_STYLE, (currentStyle & ~(int)NativeMethods.WindowStyles.WS_BORDER & ~(int)NativeMethods.WindowStyles.WS_SIZEBOX));
        }

        private void MyProcess_Exited(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.FindControlParent<TabItemConsoleCommand>().Dispose();
            });
        }

        public void SetFocus()
        {
            Task.Run(() =>
            {
                Thread.Sleep(100);
                NativeMethods.SetForegroundWindow(_srcPtr);
                User32.SetCapture(_srcPtr);
                User32.SetFocus(_srcPtr);
                User32.SetActiveWindow(_srcPtr);
                User32.EnableWindow(_srcPtr, 1);
            });
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_srcPtr != IntPtr.Zero)
                User32.SetWindowPos(_srcPtr, IntPtr.Zero, OFFSET_X + (int)this.FindVisualParent<TabExplorerBrowser>().GetVisualOffset().X, OFFSET_Y, (int)ActualWidth - OFFSET_X, (int)ActualHeight - OFFSET_SIZE_HEIGHT, User32.SWP.SHOWWINDOW);
        }

        public void Show()
        {
            NativeMethods.ShowWindow(_srcPtr, NativeMethods.WindowShowStyle.ShowNormal);
            SetFocus();
        }

        public void Hide()
        {
            NativeMethods.ShowWindow(_srcPtr, NativeMethods.WindowShowStyle.Hide);
        }

        #region IDisposable

        private bool disposedValue;

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
                    _myProcess.Kill();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}