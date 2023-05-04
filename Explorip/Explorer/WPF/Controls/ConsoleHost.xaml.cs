﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

using Explorip.WinAPI;

using ManagedShell.Interop;

namespace Explorip.Explorer.WPF.Controls
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
        private IntPtr _pointeurSource, _pointeurDestination;

        public ConsoleHost()
        {
            InitializeComponent();
            _pointeurDestination = IntPtr.Zero;
        }

        public void StartProcess(string commandline)
        {
            _pointeurDestination = (PresentationSource.FromVisual(this) as HwndSource).Handle;
            _myProcess = new Process();
            _myProcess.StartInfo.FileName = commandline;
            _myProcess.StartInfo.UseShellExecute = false;
            _myProcess.Start();
            while (_myProcess.MainWindowHandle == IntPtr.Zero) { /* Nothing to do, waiting process start */ }
            _pointeurSource = _myProcess.MainWindowHandle;
            User32.SetParent(_pointeurSource, _pointeurDestination);
            User32.SetWindowPos(_pointeurSource, IntPtr.Zero, OFFSET_X, OFFSET_Y, (int)ActualWidth - OFFSET_X, (int)ActualHeight - OFFSET_SIZE_HEIGHT, User32.SWP.NOACTIVATE);
            int currentStyle = NativeMethods.GetWindowLong(_pointeurSource, NativeMethods.GWL_STYLE);
            NativeMethods.SetWindowLong(_pointeurSource, NativeMethods.GWL_STYLE, (currentStyle & ~(int)NativeMethods.WindowStyles.WS_BORDER & ~(int)NativeMethods.WindowStyles.WS_SIZEBOX));
        }

        public void SetFocus()
        {
            User32.SetFocus(_pointeurSource);
            NativeMethods.SendMessage(_pointeurSource, (int)NativeMethods.WM.SETFOCUS, IntPtr.Zero, IntPtr.Zero);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_pointeurDestination != IntPtr.Zero)
                User32.SetWindowPos(_pointeurSource, IntPtr.Zero, OFFSET_X, OFFSET_Y, (int)ActualWidth - OFFSET_X, (int)ActualHeight - OFFSET_SIZE_HEIGHT, User32.SWP.NOACTIVATE);
        }

        public void Show()
        {
            NativeMethods.ShowWindow(_pointeurSource, NativeMethods.WindowShowStyle.ShowNormal);
        }

        public void Hide()
        {
            NativeMethods.ShowWindow(_pointeurSource, NativeMethods.WindowShowStyle.Hide);
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