using System;
using System.IO;
using System.Windows;

using Explorip.Explorer.Windows;

using ExploripApi;

using Microsoft.WindowsAPICodePack.Shell;

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

            });
        }
    }
}
