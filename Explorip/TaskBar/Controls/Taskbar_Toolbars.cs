using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.Plugins;

using ExploripConfig.Configuration;

using ExploripPlugins;

using ManagedShell.AppBar;

using Microsoft.WindowsAPICodePack.Shell.CommonFileDialogs;

namespace Explorip.TaskBar.Controls;

public partial class Taskbar
{
    private void AddToolbars()
    {
        string[] listToolbars = ConfigManager.ToolbarsPath;
        if (listToolbars?.Length > 0)
        {
            foreach (string path in listToolbars.Where(p => ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(p)))
            {
                if (Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
                    AddToolbar(path, false);
                else if (Guid.TryParse(path, out Guid guidPlugin))
                {
                    IExploripToolbar plugin = PluginsManager.GetPlugin(guidPlugin);
                    if (plugin != null)
                        AddToolbar(plugin, false);
                }
            }
        }
    }

    #region Manage toolbar

    public Grid ListToolbars
    {
        get { return ToolsBars; }
    }

    public void RefreshAllInvisibleIcons()
    {
        System.Threading.Tasks.Task.Run(async () =>
        {
            await System.Threading.Tasks.Task.Delay(1000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ToolsBars.Children.OfType<Toolbar>().ToList().ForEach(tb => tb.DataContext.UpdateInvisibleIcons());
            });
        });
    }

    /// <summary>
    /// Add toolbar (like QuickLaunch)
    /// </summary>
    /// <param name="path">Path/Directory where shortcuts to display are stored</param>
    /// <param name="resize">The Taskbar must be resized ?</param>
    public Toolbar AddToolbar(string path, bool resize = true)
    {
        if (!Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
            return null;
        AddBaseGrid(0, 0);
        int maxWidth = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarMaxWidth(path);
        (int, int) gridPos = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarGrid(path);
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
            AddMissingGrid(gridPos.Item1 + 1, gridPos.Item2 + 1, 0, 22);
        else if (ToolsBars.Children.Count > 0)
            AddMissingGrid(ToolsBars.ColumnDefinitions.Count, ToolsBars.RowDefinitions.Count + 1, 0, 0);
        Toolbar newToolbar = new();
        newToolbar.DataContext.Path = path;
        if (maxWidth > 0)
            newToolbar.MaxWidth = maxWidth;
        Grid.SetRow(newToolbar, (gridPos.Item2 < 0 ? ToolsBars.RowDefinitions.Count - 1 : gridPos.Item2));
        Grid.SetColumn(newToolbar, (gridPos.Item1 < 0 ? 0 : gridPos.Item1));
        ToolsBars.Children.Add(newToolbar);
        if (resize)
        {
            if (AppBarEdge == AppBarEdge.Top || AppBarEdge == AppBarEdge.Bottom)
            {
                Height += 22;
                DesiredHeight = Height;
            }
            else
            {
                Width += 22;
                DesiredWidth = Width;
            }
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(path, true);
        }
        _appBarManager.SetWorkArea(Screen);
        Panel.SetZIndex(newToolbar, ConfigManager.GetTaskbarConfig(NumScreen).ToolbarZIndex(path));
        return newToolbar;
    }

    /// <summary>
    /// Add a plugin Toolbar
    /// </summary>
    /// <param name="plugin">Plugin to add</param>
    /// <param name="resize">The Taskbar must be resized ?</param>
    public void AddToolbar(IExploripToolbar plugin, bool resize = true)
    {
        double height = plugin.MinHeight;
        double width = plugin.MinWidth;
        ToolbarPlugin tp = new();
        Grid.SetColumn(plugin.ExploripToolbar, 1);
        tp.MainGrid.Children.Add(plugin.ExploripToolbar);
        tp.PluginLinked = plugin;
        AddBaseGrid(width, height);
        (int, int) gridPos = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarGrid(plugin.GuidKey.ToString());
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
            AddMissingGrid(gridPos.Item1 + 1, gridPos.Item2 + 1, 0, height);
        else if (ToolsBars.Children.Count > 0)
            AddMissingGrid(ToolsBars.ColumnDefinitions.Count, ToolsBars.RowDefinitions.Count + 1, plugin.MinWidth, plugin.MinHeight);
        int numRow = (gridPos.Item2 < 0 ? ToolsBars.RowDefinitions.Count - 1 : gridPos.Item2);
        int numColumn = (gridPos.Item1 < 0 ? 0 : gridPos.Item1);
        int maxWidth = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarMaxWidth(plugin.GuidKey.ToString());
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
        {
            ToolsBars.RowDefinitions[numRow].Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel));
            ToolsBars.ColumnDefinitions[numColumn].Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel));
            if (maxWidth > 0)
                ToolsBars.ColumnDefinitions[numColumn].MaxWidth = maxWidth;
        }
        Grid.SetRow(tp, numRow);
        Grid.SetColumn(tp, numColumn);
        ToolsBars.Children.Add(tp);
        if (resize)
        {
            if (AppBarEdge == AppBarEdge.Top || AppBarEdge == AppBarEdge.Bottom)
            {
                Height += height > 0 ? height : plugin.ExploripToolbar.ActualHeight;
                DesiredHeight = Height;
            }
            else
            {
                Width += width > 0 ? width : plugin.ExploripToolbar.ActualWidth;
                DesiredWidth = width;
            }
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(tp.DataContext.Id, true);
        }
        _appBarManager.SetWorkArea(Screen);
        plugin.SetGlobalColors(ExploripSharedCopy.Constants.Colors.BackgroundColorBrush, ExploripSharedCopy.Constants.Colors.ForegroundColorBrush, ExploripSharedCopy.Constants.Colors.AccentColorBrush);
        plugin.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
        Panel.SetZIndex(tp, ConfigManager.GetTaskbarConfig(NumScreen).ToolbarZIndex(plugin.GuidKey.ToString()));
    }

    private void AddBaseGrid(double width, double height)
    {
        if (ToolsBars.ColumnDefinitions.Count == 0)
            ToolsBars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
        if (ToolsBars.RowDefinitions.Count == 0)
            ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
    }

    private void AddMissingGrid(int numColumn, int numRow, double width, double height)
    {
        while (numColumn > 0 && ToolsBars.ColumnDefinitions.Count < numColumn)
            ToolsBars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
        while (numRow > 0 && ToolsBars.RowDefinitions.Count < numRow)
            ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
    }

    private void AddToolbar_Click(object sender, RoutedEventArgs e)
    {
        CommonOpenFileDialog dialog = new()
        {
            IsFolderPicker = true,
        };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            Toolbar newTb = AddToolbar(dialog.FileName);
            if (newTb != null)
            {
                ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<BaseToolbar>().Select(tb => tb.BaseDataContext.Id)];
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
                newTb.DataContext.ShowHideTitle();
                newTb.DataContext.CurrentShowLargeIcon = true;
                newTb.DataContext.ShowLargeIcon();
            }
        }
    }

    private Point _lastMousePosition;
    private void ShowHideTitleToolbar_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.DataContext.ShowHideTitle();
        }
    }

    private void ShowSmallLargeIcon_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.DataContext.ShowLargeIcon();
        }
    }

    private void ToolbarListPlugins_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is MenuItem mi)
        {
            string pluginName = mi.Header.ToString();
            if (pluginName == Constants.Localization.PLUGINS || pluginName == Constants.Localization.NO_PLUGINS)
                return;
            if (pluginName == Constants.Localization.RELOAD_PLUGINS)
            {
                ReloadPlugins();
            }
            else if (mi.Tag is Guid guidKey)
            {
                IExploripToolbar plugin = PluginsManager.GetPlugin(guidKey);
                AddToolbar(plugin);
                if (MainScreen)
                    ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<BaseToolbar>().Select(tb => tb.BaseDataContext.Id)];
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
            }
        }
    }

    private void ReloadPlugins()
    {
        PluginsManager.Reload();
        List<ToolbarPlugin> listToRemove = [];
        List<(ToolbarPlugin, IExploripToolbar)> listToReplace = [];
        foreach (ToolbarPlugin tp in ToolsBars.Children.OfType<ToolbarPlugin>())
        {
            IExploripToolbar toolbar = PluginsManager.ListPlugins().FirstOrDefault(i => i.GuidKey == tp.PluginLinked.GuidKey);
            if (toolbar == null)
                listToRemove.Add(tp);
            else if (toolbar.Version.CompareTo(tp.PluginLinked.Version) > 0)
                listToReplace.Add((tp, toolbar));
        }
        foreach (ToolbarPlugin tp in listToRemove)
            ToolsBars.Children.Remove(tp);
        foreach ((ToolbarPlugin, IExploripToolbar) tp in listToReplace)
        {
            tp.Item1.MainGrid.Children.Clear();
            tp.Item1.MainGrid.Children.Add(tp.Item2.ExploripToolbar);
            tp.Item1.PluginLinked = tp.Item2;
        }
    }

    private void UpdatePlugins()
    {
        foreach (ToolbarPlugin plugin in ToolsBars.Children.OfType<ToolbarPlugin>())
            plugin.PluginLinked.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
    }

    #endregion
}
