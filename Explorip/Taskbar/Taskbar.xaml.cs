﻿using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Explorip.TaskBar.Utilities;
using Application = System.Windows.Application;
using Explorip.TaskBar.Controls;

namespace Explorip.TaskBar
{
    /// <summary>
    /// Interaction logic for Taskbar.xaml
    /// </summary>
    public partial class Taskbar : AppBarWindow
    {
        private bool _isReopening;
        private readonly bool _mainScreen;

        public Taskbar(StartMenuMonitor startMenuMonitor, AppBarScreen screen, AppBarEdge edge)
            : base(MyApp.MonShellManager.AppBarManager, MyApp.MonShellManager.ExplorerHelper, MyApp.MonShellManager.FullScreenHelper, screen, edge, 0)
        {
            InitializeComponent();

            _mainScreen = screen.Primary;
            DataContext = MyApp.MonShellManager;
            StartButton.StartMenuMonitor = startMenuMonitor;

            DesiredHeight = Application.Current.FindResource("TaskbarHeight") as double? ?? 0;
            DesiredWidth = Application.Current.FindResource("TaskbarWidth") as double? ?? 0;

            AllowsTransparency = Application.Current.FindResource("AllowsTransparency") as bool? ?? false;
            SetFontSmoothing();

            _explorerHelper.HideExplorerTaskbar = true;

            Settings.Instance.PropertyChanged += Settings_PropertyChanged;

            // Layout rounding causes incorrect sizing on non-integer scales
            if (DpiHelper.DpiScale % 1 != 0) UseLayoutRounding = false;

            if (Settings.Instance.ShowQuickLaunch)
            {
                QuickLaunchToolbar.Visibility = Visibility.Visible;
                DesiredHeight += 16;
            }
        }

        public bool MainScreen
        {
            get { return _mainScreen; }
        }

        protected override void OnSourceInitialized(object sender, EventArgs e)
        {
            base.OnSourceInitialized(sender, e);

            SetBlur(AllowsTransparency);
        }
        
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            base.WndProc(hwnd, msg, wParam, lParam, ref handled);

            if ((msg == (int)NativeMethods.WM.SYSCOLORCHANGE || 
                    msg == (int)NativeMethods.WM.SETTINGCHANGE) && 
                Settings.Instance.Theme == DictionaryManager.THEME_DEFAULT)
            {
                handled = true;

                // If the color scheme changes, re-apply the current theme to get updated colors.
                ((MyApp)Application.Current).DictionaryManager.SetThemeFromSettings();
            }

            return IntPtr.Zero;
        }

        public override void SetPosition()
        {
            base.SetPosition();

            MyApp.MonShellManager.NotificationArea.SetTrayHostSizeData(new TrayHostSizeData
            {
                edge = (NativeMethods.ABEdge)AppBarEdge,
                rc = new NativeMethods.Rect
                {
                    Top = (int) (Top * DpiScale),
                    Left = (int) (Left * DpiScale),
                    Bottom = (int) ((Top + Height) * DpiScale),
                    Right = (int) ((Left + Width) * DpiScale)
                }
            });
        }

        private void SetFontSmoothing()
        {
            VisualTextRenderingMode = Settings.Instance.AllowFontSmoothing ? TextRenderingMode.Auto : TextRenderingMode.Aliased;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Theme")
            {
                bool newTransparency = Application.Current.FindResource("AllowsTransparency") as bool? ?? false;
                double newHeight = Application.Current.FindResource("TaskbarHeight") as double? ?? 0;
                double newWidth = Application.Current.FindResource("TaskbarWidth") as double? ?? 0;
                bool heightChanged = newHeight != DesiredHeight;
                bool widthChanged = newWidth != DesiredWidth;

                if (AllowsTransparency != newTransparency)
                {
                    // Transparency cannot be changed on an open window.
                    _isReopening = true;
                    ((MyApp)Application.Current).ReopenTaskbar();
                    return;
                }

                DesiredHeight = newHeight;
                DesiredWidth = newWidth;

                if (Orientation == Orientation.Horizontal && heightChanged)
                {
                    Height = DesiredHeight;
                    SetScreenPosition();
                }
                else if (Orientation == Orientation.Vertical && widthChanged)
                {
                    Width = DesiredWidth;
                    SetScreenPosition();
                }
            }
            else if (e.PropertyName == "AllowFontSmoothing")
            {
                SetFontSmoothing();
            }
            else if (e.PropertyName == "ShowQuickLaunch")
            {
                if (Settings.Instance.ShowQuickLaunch)
                {
                    QuickLaunchToolbar.Visibility = Visibility.Visible;
                    DesiredHeight += 16;
                }
                else
                {
                    QuickLaunchToolbar.Visibility = Visibility.Collapsed;
                    DesiredHeight -= 16;
                }
            }
            else if (e.PropertyName == "Edge")
            {
                AppBarEdge = (AppBarEdge)Settings.Instance.Edge;
                SetScreenPosition();
            }
        }

        private void Taskbar_OnLocationChanged(object sender, EventArgs e)
        {
            // primarily for win7/8, they will set up the appbar correctly but then put it in the wrong place
            if (Orientation == Orientation.Vertical)
            {
                double desiredLeft = 0;

                if (AppBarEdge == AppBarEdge.Right)
                {
                    desiredLeft = Screen.Bounds.Right / DpiScale - Width;
                }

                if (Left != desiredLeft) Left = desiredLeft;
            }
            else
            {
                double desiredTop = 0;

                if (AppBarEdge == AppBarEdge.Bottom)
                {
                    desiredTop = Screen.Bounds.Bottom / DpiScale - Height;
                }

                if (Top != desiredTop) Top = desiredTop;
            }
        }

        private void TaskManagerMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ShellHelper.StartTaskManager();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ((MyApp)Application.Current).ExitGracefully();
        }

        protected override void CustomClosing()
        {
            if (AllowClose)
            {
                if (!_isReopening) _explorerHelper.HideExplorerTaskbar = false;
                QuickLaunchToolbar.Visibility = Visibility.Collapsed;
                
                Settings.Instance.PropertyChanged -= Settings_PropertyChanged;
            }
        }

        private void PropertiesMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            PropertiesWindow.Open(((MyApp)Application.Current).DictionaryManager);
        }

        private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_mainScreen)
                WindowsDesktop.VirtualDesktopProvider.Default.Initialize().Wait();
            MyApp.MonShellManager.Tasks.Initialize(new TaskCategoryProvider());
        }
    }
}
