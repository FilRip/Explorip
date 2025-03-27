using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Explorip.StartMenu.ViewModels;
using Explorip.TaskBar;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using Securify.ShellLink;

using WpfScreenHelper;

namespace Explorip.StartMenu.Window
{
    /// <summary>
    /// Logique d'interaction pour StartMenuWindow.xaml
    /// </summary>
    public partial class StartMenuWindow : System.Windows.Window
    {
        private IntPtr _keyboardHookPtr, _windowsStartMenu;

        public StartMenuWindow()
        {
            InitializeComponent();

            MyDataContext.HideWindow = Hide;
            MyDataContext.ShowWindow = Show;

            HideWindowsStartMenu();
            SetMyStartMenu(this);
            HookWinKey();
            Application.Current.Deactivated += Current_Deactivated;
            if (ConfigManager.StartMenuBackground != null)
                Background = ConfigManager.StartMenuBackground;
            if (MyStartMenuApp.MyShellManager == null)
                MyTaskbarApp.MyShellManager.TasksService.WindowActivated += TasksService_WindowActivated;
            else
                MyStartMenuApp.MyShellManager.TasksService.WindowActivated += TasksService_WindowActivated;
        }

        public static StartMenuWindow MyStartMenu { get; private set; }

        private static void SetMyStartMenu(StartMenuWindow startMenuWindow)
        {
            MyStartMenu = startMenuWindow;
        }

        public StartMenuViewModel MyDataContext
        {
            get { return (StartMenuViewModel)DataContext; }
        }

        #region Events

        private void ScrollViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void ScrollViewer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        #endregion

        #region Hook Win key press

        private void TasksService_WindowActivated(IntPtr windowHandle)
        {
            if (IsVisible && windowHandle != IntPtr.Zero)
            {
                if (_waitForOpen != null && _waitForOpen.ElapsedMilliseconds < 200)
                    return;
                NativeMethods.GetWindowThreadProcessId(windowHandle, out uint pid);
                if (pid != Process.GetCurrentProcess().Id)
                    Hide();
            }
        }

        private void HookWinKey()
        {
            _keyboardHookPtr = NativeMethods.SetWindowsHookEx(NativeMethods.HookType.WH_KEYBOARD_LL, MyKeyboardHook, IntPtr.Zero, 0);
        }

        private static int _lastPressedKey;
        private static Stopwatch _waitForOpen;

        private void Current_Deactivated(object sender, EventArgs e)
        {
#if DEBUG
            Debug.WriteLine("LOST APPLICATION ACTIVATION");
#endif
            if (_waitForOpen != null && _waitForOpen.ElapsedMilliseconds < 200)
                return;
            MyDataContext.HideAllContextMenu();
            Hide();
        }

        private static int MyKeyboardHook(int code, int wParam, ref NativeMethods.KeyboardHookStruct lParam)
        {
            if (code >= 0 && wParam == (int)NativeMethods.WM.KEYDOWN)
            {
                _lastPressedKey = lParam.vkCode;
            }
            else if (code >= 0 && wParam == (int)NativeMethods.WM.KEYUP &&
                _lastPressedKey == lParam.vkCode && (lParam.vkCode == (int)NativeMethods.VK.LWIN || lParam.vkCode == (int)NativeMethods.VK.RWIN))
            {
                if (MyStartMenu.IsVisible)
                    MyStartMenu.Hide();
                else
                {
                    _waitForOpen = Stopwatch.StartNew();
                    MyStartMenu.Show();
                    MyStartMenu.Activate();
                }
            }
            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                System.Drawing.Point p = new();
                NativeMethods.GetCursorPos(ref p);
                Screen screen = Screen.FromPoint(new Point(p.X, p.Y));
                Left = screen.WorkingArea.X;
                Top = (int)(screen.WorkingArea.Bottom / screen.ScaleFactor) - Height;
            }
        }

        private void HideWindowsStartMenu()
        {
            _windowsStartMenu = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.START);
            if (_windowsStartMenu != IntPtr.Zero)
                NativeMethods.ShowWindow(_windowsStartMenu, NativeMethods.WindowShowStyle.Hide);
        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            if (_keyboardHookPtr != IntPtr.Zero)
                NativeMethods.UnhookWindowsHookEx(_keyboardHookPtr);
            if (_windowsStartMenu != IntPtr.Zero)
                NativeMethods.ShowWindow(_windowsStartMenu, NativeMethods.WindowShowStyle.ShowNormal);
            Application.Current.Deactivated -= Current_Deactivated;
        }
    }
}
