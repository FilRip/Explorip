using System;
using System.Windows;

using Explorip.Explorer.Windows;

using ExploripApi;

using ExploripSharedCopy.Controls;

namespace Explorip.Helpers;

[Serializable()]
public sealed class IpcServer : IServerIpc
{
    public void ReceivedNewWindow(string[] args)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            WpfExplorerBrowser newExplorerWindow = new(args, false);
            newExplorerWindow.LeftTab.AddNewTab(args[0]);
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
            win.Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
            win.Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
            win.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
            win.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
        }

        ExploripSharedCopy.Helpers.CreateOperations.CreateFolder(path, name, SetConstants);
    }

    public void CreateShortcut(string path, string name)
    {
        ExploripSharedCopy.Helpers.CreateOperations.CreateShortcut(path, name);
    }

    public void Ping()
    {
        // Nothing to do here, just keep ipc channel on line
    }
}
