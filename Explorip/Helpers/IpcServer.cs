using System;
using System.Windows;

using Explorip.Explorer.Windows;

using ExploripApi;

using ExploripSharedCopy.Controls;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Helpers;

[Serializable()]
public sealed class IpcServer : IServerIpc
{
    public void ReceivedNewWindow(string[] args)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            WpfExplorerBrowser newExplorerWindow = new(false);
            newExplorerWindow.LeftTab.FirstTab.Navigation(ShellObject.FromParsingName(args[0]));
            newExplorerWindow.RightTab.CloseAllTabs();
            newExplorerWindow.RightTab.HideTab();
            newExplorerWindow.Show();
        });
    }

    public void CreateFolder(string path, string name)
    {
        static void SetConstants(InputBoxWindow win)
        {
            win.Title = Constants.Localization.CREATE_FOLDER;
            win.Icon = Constants.Icons.Folder;
            win.Background = Constants.Colors.BackgroundColorBrush;
            win.Foreground = Constants.Colors.ForegroundColorBrush;
            win.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
            win.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
        }

        ExploripSharedCopy.Helpers.CreateOperations.CreateFolder(path, name, SetConstants);
    }

    public void CreateShortcut(string path, string name)
    {
        static void SetConstants(CreateShortcutWindow win)
        {
            win.Title = Constants.Localization.CREATE_SHORTCUT;
            win.Icon = Constants.Icons.Shortcut;
            win.Background = Constants.Colors.BackgroundColorBrush;
            win.Foreground = Constants.Colors.ForegroundColorBrush;
            win.SetQuestions(Constants.Localization.CREATE_SHORTCUT_Q1, Constants.Localization.CREATE_SHORTCUT_Q2);
            win.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
            win.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
            win.SetBrowse(Constants.Localization.BROWSE);
        }

        ExploripSharedCopy.Helpers.CreateOperations.CreateShortcut(path, name, SetConstants);
    }

    public void Ping()
    {
        // Nothing to do here, just keep ipc channel on line
    }
}
