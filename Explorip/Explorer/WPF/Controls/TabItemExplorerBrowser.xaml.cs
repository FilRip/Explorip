using Explorip.Explorer.WPF.ViewModels;

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemExplorerBrowser.xaml
    /// </summary>
    public partial class TabItemExplorerBrowser : TabItem
    {
        public TabItemExplorerBrowser()
        {
            InitializeComponent();
            ExplorerBrowser.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
        }

        public TabItemExplorerBrowserViewModel MyDataContext
        {
            get { return (TabItemExplorerBrowserViewModel)DataContext; }
        }

        private void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            MyDataContext.TabTitle = e.NewLocation.Name;
            CurrentPath.Inlines?.Clear();

            string pathLink = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
            StringBuilder partialPath = new();
            foreach (string path in pathLink.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries))
            {
                partialPath.Append(path + @"\");
                Hyperlink lb = new()
                {
                    Foreground = Brushes.Yellow,
                    NavigateUri = new Uri(partialPath.ToString()),
                };
                lb.RequestNavigate += Lb_RequestNavigate;
                lb.Inlines.Add(path);
                CurrentPath.Inlines.Add(lb);
                CurrentPath.Inlines.Add("\\");
            }
        }

        private void Lb_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(e.Uri.ToString()));
        }

        public void Navigation(string repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(repertoire));
        }
    }
}
