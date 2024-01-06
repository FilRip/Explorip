using System;
using System.IO;
using System.Windows;

using Explorip.Explorer.Windows;

using ExploripApi;

using ExploripSharedCopy.Controls;

using Microsoft.WindowsAPICodePack.Shell;

using Securify.ShellLink;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.Helpers
{
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
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                InputBoxWindow input = new()
                {
                    CheckValidPathName = true,
                    Title = Constants.Localization.CREATE_FOLDER,
                    Question = path,
                    UserEdit = name,
                    Icon = Constants.Icons.Folder,
                    Background = Constants.Colors.BackgroundColorBrush,
                    Foreground = Constants.Colors.ForegroundColorBrush,
                };
                input.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
                input.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
                if (input.ShowDialog() == true)
                {
                    Directory.CreateDirectory(Path.Combine(path, input.UserEdit));
                }
            });
        }

        public void CreateShortcut(string path, string name)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CreateShortcutWindow win = new()
                {
                    Title = Constants.Localization.CREATE_SHORTCUT,
                    Icon = Constants.Icons.Shortcut,
                    Background = Constants.Colors.BackgroundColorBrush,
                    Foreground = Constants.Colors.ForegroundColorBrush,
                };
                win.SetQuestions(Constants.Localization.CREATE_SHORTCUT_Q1, Constants.Localization.CREATE_SHORTCUT_Q2);
                win.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
                win.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
                win.SetBrowse(Constants.Localization.BROWSE);
                if (win.ShowDialog() == true)
                {
                    Shortcut sc = Shortcut.CreateShortcut(win.Target);
                    sc.WriteToFile(Path.Combine(path, win.ShortcutName + ".lnk"));
                }
            });
        }

        public void Ping()
        {
            // Nothing to do here, just keep ipc channel on line
        }
    }
}
