﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Explorip.TaskBar.Utilities;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.Constants;

namespace Explorip.TaskBar
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        private static PropertiesWindow _instance;

        private readonly DictionaryManager _dictionaryManager;

        private PropertiesWindow(DictionaryManager dictionaryManager)
        {
            _dictionaryManager = dictionaryManager;

            InitializeComponent();

            LoadAutoStart();
            LoadLanguages();
            LoadThemes();
        }

        public static void Open(DictionaryManager dictionaryManager)
        {
            if (_instance == null)
            {
                _instance = new PropertiesWindow(dictionaryManager);
                _instance.Show();
            }
            else
            {
                _instance.Activate();
            }
        }

        private void LoadAutoStart()
        {
            try
            {
                RegistryKey rKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
                List<string> rKeyValueNames = rKey?.GetValueNames().ToList();

                if (rKeyValueNames != null)
                {
                    if (rKeyValueNames.Contains("Explorip"))
                    {
                        AutoStartCheckBox.IsChecked = true;
                    }
                    else
                    {
                        AutoStartCheckBox.IsChecked = false;
                    }
                }
            }
            catch (Exception e)
            {
                ShellLogger.Error($"PropertiesWindow: Unable to load autorun setting from registry: {e.Message}");
            }
        }

        private void LoadLanguages()
        {
            foreach (string language in _dictionaryManager.GetLanguages())
            {
                cboLanguageSelect.Items.Add(language);
            }
        }

        private void LoadThemes()
        {
            foreach (string theme in _dictionaryManager.GetThemes())
            {
                cboThemeSelect.Items.Add(theme);
            }
        }

        private void UpdateWindowPosition()
        {
            switch (Settings.Instance.Edge)
            {
                case (int)AppBarEdge.Left:
                case (int)AppBarEdge.Top:
                    Left = (SystemInformations.WorkingArea.Left / DpiHelper.DpiScale) + 10;
                    Top = (SystemInformations.WorkingArea.Top / DpiHelper.DpiScale) + 10;
                    break;
                case (int)AppBarEdge.Right:
                    Left = (SystemInformations.WorkingArea.Right / DpiHelper.DpiScale) - Width - 10;
                    Top = (SystemInformations.WorkingArea.Top / DpiHelper.DpiScale) + 10;
                    break;
                case (int)AppBarEdge.Bottom:
                    Left = (SystemInformations.WorkingArea.Left / DpiHelper.DpiScale) + 10;
                    Top = (SystemInformations.WorkingArea.Bottom / DpiHelper.DpiScale) - Height - 10;
                    break;
            }
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetQuickLaunchLocation_OnClick(object sender, RoutedEventArgs e)
        {
            /*FolderBrowserDialog fbd = new()
            {
                Description = (string)FindResource("quick_launch_folder"),
                ShowNewFolderButton = false,
                SelectedPath = Settings.Instance.QuickLaunchPath
            };

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Instance.QuickLaunchPath = fbd.SelectedPath;
            }*/
        }

        private static void RecycleInstance()
        {
            _instance = null;
        }
        private void PropertiesWindow_OnClosing(object sender, CancelEventArgs e)
        {
            RecycleInstance();
        }

        private void PropertiesWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = 10;
            Top = (ScreenHelper.PrimaryMonitorDeviceSize.Height / DpiHelper.DpiScale) - Height - 40;
            UpdateWindowPosition();
        }

        private void PropertiesWindow_OnContentRendered(object sender, EventArgs e)
        {
            UpdateWindowPosition();
        }

        private void AutoStartCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey rKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                System.Windows.Controls.CheckBox chkBox = (System.Windows.Controls.CheckBox)sender;

                if (chkBox.IsChecked.Equals(false))
                {
                    rKey?.DeleteValue("Explorip");
                }
                else
                {
                    rKey?.SetValue("Explorip", ExePath.GetExecutablePath());
                }
            }
            catch (Exception exception)
            {
                ShellLogger.Error($"PropertiesWindow: Unable to update registry autorun setting: {exception.Message}");
            }
        }

        private void CboEdgeSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cboEdgeSelect.SelectedItem == null)
            {
                cboEdgeSelect.SelectedValue = cboEdgeSelect.Items[Settings.Instance.Edge];
            }
        }
    }
}
