using System;
using System.IO;
using System.Windows;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Windows;

using ExploripApi;

using Microsoft.WindowsAPICodePack.Shell;

using Securify.ShellLink;

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
                };
                input.TxtQuestion.Text = path;
                input.TxtUserEdit.Text = name;
                if (input.ShowDialog() == true)
                {
                    Directory.CreateDirectory(Path.Combine(path, input.TxtUserEdit.Text));
                }
            });
        }

        public void CreateShortcut(string path, string name)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CreateShortcutWindow win = new();
                if (win.ShowDialog() == true)
                {
                    Shortcut sc = Shortcut.CreateShortcut(win.TxtTarget.Text);
                    sc.WriteToFile(Path.Combine(path, win.TxtName.Text + ".lnk"));
                }
            });
        }

        public void Ping()
        {
            // Nothing to do here, just keep ipc channel on line
        }
    }
}
