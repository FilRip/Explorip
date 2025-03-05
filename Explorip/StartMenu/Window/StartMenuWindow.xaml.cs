﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using Explorip.Helpers;
using Explorip.StartMenu.ViewModels;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.StartMenu.Window
{
    /// <summary>
    /// Logique d'interaction pour StartMenuWindow.xaml
    /// </summary>
    public partial class StartMenuWindow : System.Windows.Window
    {
        private readonly ContextMenu _cmUser, _cmStart;
        private IntPtr _keyboardHookPtr, _windowsStartMenu;
        public static StartMenuWindow MyStartMenu { get; private set; }

        public StartMenuWindow()
        {
            InitializeComponent();

            string xaml = "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:colors='clr-namespace:ExploripSharedCopy.Constants;assembly=ExploripSharedCopy'><StackPanel Margin=\"-20,0,0,0\" Background=\"{x:Static colors:Colors.BackgroundColorBrush}\"/></ItemsPanelTemplate>";
            ItemsPanelTemplate itp = XamlReader.Parse(xaml) as ItemsPanelTemplate;

            _cmUser = new()
            {
                Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                ItemsPanel = itp,
            };
            _cmUser.AddEntry(Constants.Localization.LOCK, ShellHelper.Lock);
            _cmUser.AddEntry(Constants.Localization.DISCONNECT, ShellHelper.Logoff);

            _cmStart = new()
            {
                Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                ItemsPanel = itp,
            };
            _cmStart.AddEntry(Constants.Localization.PUT_HYBERNATE, Hybernate);
            _cmStart.AddEntry(Constants.Localization.SHUTDOWN, Shutdown);
            _cmStart.AddEntry(Constants.Localization.RESTART, Restart);

            MyDataContext.HideWindow = Hide;
            MyDataContext.ShowWindow = Show;

#if DEBUG
            Topmost = false;
            ShowInTaskbar = true;
#endif
            HideWindowsStartMenu();
            SetMyStartMenu(this);
            HookWinKey();
        }

        private static void SetMyStartMenu(StartMenuWindow startMenuWindow)
        {
            MyStartMenu = startMenuWindow;
        }

        public StartMenuViewModel MyDataContext
        {
            get { return (StartMenuViewModel)DataContext; }
        }

        private static void Hybernate()
        {
            ShellHelper.StartProcess("shutdown /h", hidden: true);
        }

        private static void Shutdown()
        {
            ShellHelper.StartProcess("shutdown /s /t 0", hidden: true);
        }

        private static void Restart()
        {
            ShellHelper.StartProcess("shutdown /r /t 0", hidden: true);
        }

        private void StartMenuWindow_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ScrollViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void ScrollViewer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void ParamButton_Click(object sender, RoutedEventArgs e)
        {
            ShellHelper.ShowConfigPanel();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _cmStart.IsOpen = true;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            _cmUser.IsOpen = true;
        }

        private void HookWinKey()
        {
            _keyboardHookPtr = NativeMethods.SetWindowsHookEx(NativeMethods.HookType.WH_KEYBOARD_LL, MyKeyboardHook, IntPtr.Zero, 0);
        }

        private static int _lastPressedKey;
        private static int MyKeyboardHook(int code, int wParam, ref NativeMethods.KeyboardHookStruct lParam)
        {
            if (code >= 0 && wParam == (int)NativeMethods.WM.KEYDOWN)
            {
                _lastPressedKey = lParam.vkCode;
            }
            else if (code >=0 && wParam == (int)NativeMethods.WM.KEYUP &&
                _lastPressedKey == lParam.vkCode && (lParam.vkCode == (int)NativeMethods.VK.LWIN || lParam.vkCode == (int)NativeMethods.VK.RWIN))
            {
                if (MyStartMenu.IsVisible)
                    MyStartMenu.Hide();
                else
                {
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
                Top = (int)screen.WorkingArea.Bottom - Height;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_keyboardHookPtr != IntPtr.Zero)
                NativeMethods.UnhookWindowsHookEx(_keyboardHookPtr);
            if (_windowsStartMenu != IntPtr.Zero)
                NativeMethods.ShowWindow(_windowsStartMenu, NativeMethods.WindowShowStyle.ShowNormal);
        }

        private void HideWindowsStartMenu()
        {
            _windowsStartMenu = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.START);
            if (_windowsStartMenu != IntPtr.Zero)
                NativeMethods.ShowWindow(_windowsStartMenu, NativeMethods.WindowShowStyle.Hide);
        }
    }
}
