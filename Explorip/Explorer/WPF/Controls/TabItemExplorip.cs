using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Explorer.WPF.Controls
{
    public abstract class TabItemExplorip : TabItem, IDisposable
    {
        protected void InitializeExplorip()
        {
            AllowDrop = true;
            HeaderWithCloseButton closableTabHeader = new();
            Header = closableTabHeader;
            MyHeader.DragOver += MyHeader_DragOver;
            closableTabHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;
            Drop += TabItem_Drop;
            PreviewMouseMove += TabItem_PreviewMouseMove;
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonClose.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 0, 0);
        }

        /// <summary>
        /// Property - Set the Title of the Tab
        /// </summary>
        protected void SetTitle(string newTitle)
        {
            if (MyHeader != null)
                MyHeader.Label_TabTitle.Content = newTitle.Replace("_","__");
        }

        #region Events for selecting/deselecting

        public delegate void DelegateOnSelecting();
        public event DelegateOnSelecting OnSelecting;

        public delegate void DelegateOnDeSelecting();
        public event DelegateOnSelecting OnDeSelecting;

        public void RaiseOnSelecting()
        {
            OnSelecting?.Invoke();
        }

        public void RaiseOnDeSelecting()
        {
            OnDeSelecting?.Invoke();
        }

        #endregion

        #region Properties

        protected HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        protected TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)Parent; }
        }

        #endregion

        #region Events for Close button

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            if (MyHeader != null)
                MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            if (MyHeader != null)
                MyHeader.ButtonClose.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (MyHeader != null)
                MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (MyHeader != null && !IsSelected)
            {
                MyHeader.ButtonClose.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Drag'n Drop

        private void MyHeader_DragOver(object sender, DragEventArgs e)
        {
            TabItemExplorip tab = (TabItemExplorip)((HeaderWithCloseButton)e.Source).Parent;
            if (MyTabControl.SelectedItem != tab)
            {
                if (e.Data.GetData("FileDrop") != null)
                    MyTabControl.SelectedItem = tab;
                else
                {
                    TabItemExplorip tabItemTarget = null;
                    if (e.Source is TabItemExplorip browser)
                        tabItemTarget = browser;
                    else if (e.Source is HeaderWithCloseButton entete && entete.Parent is TabItemExplorip browser2)
                        tabItemTarget = browser2;
                    
#pragma warning disable IDE0074, IDE0270 // Utiliser une assignation composée
                    TabItemExplorip tabItemSource = (TabItemExplorerBrowser)e.Data.GetData(typeof(TabItemExplorerBrowser));
                    if (tabItemSource == null)
                        tabItemSource = (TabItemConsoleCommand)e.Data.GetData(typeof(TabItemConsoleCommand));
#pragma warning restore IDE0074, IDE0270 // Utiliser une assignation composée

                    if (tabItemTarget != null &&
                        tabItemSource != null &&
                        !tabItemTarget.Equals(tabItemSource) &&
                        tabItemTarget.Parent is TabExplorerBrowser tabControlTarget)
                    {
                        TabExplorerBrowser tabControlSource = (TabExplorerBrowser)tabItemSource.Parent;
                        int targetIndex = tabControlTarget.Items.IndexOf(tabItemTarget);

                        if (tabControlTarget == tabControlSource)
                        {
                            tabControlTarget.Items.Remove(tabItemSource);
                            tabControlTarget.Items.Insert(targetIndex, tabItemSource);
                            tabItemSource.IsSelected = true;
                        }
                    }
                }
            }
        }

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is not TabItemExplorip tabItem)
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            TabItemExplorip tabItemTarget = null;
            if (e.Source is TabItemExplorip browser)
                tabItemTarget = browser;
            else if (e.Source is HeaderWithCloseButton entete && entete.Parent is TabItemExplorip browser2)
                tabItemTarget = browser2;

#pragma warning disable IDE0074, IDE0270 // Utiliser une assignation composée
            TabItemExplorip tabItemSource = (TabItemExplorerBrowser)e.Data.GetData(typeof(TabItemExplorerBrowser));
            if (tabItemSource == null)
                tabItemSource = (TabItemConsoleCommand)e.Data.GetData(typeof(TabItemConsoleCommand));
#pragma warning restore IDE0074, IDE0270 // Utiliser une assignation composée

            if (tabItemTarget != null &&
                tabItemSource != null &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is TabExplorerBrowser tabControlTarget)
            {
                TabExplorerBrowser tabControlSource = (TabExplorerBrowser)tabItemSource.Parent;
                int targetIndex = tabControlTarget.Items.IndexOf(tabItemTarget);

                if (tabControlTarget != tabControlSource)
                {
                    if (tabControlSource.Items.Count > 1 || tabControlSource.AllowCloseLastTab)
                        tabControlSource.Items.Remove(tabItemSource);
                    else
                    {
                        ShellObject repertoire = (ShellObject)KnownFolders.Desktop;
                        tabItemSource = new TabItemExplorerBrowser();
                        ((TabItemExplorerBrowser)tabItemSource).Navigation(repertoire);
                    }
                    tabControlTarget.Items.Insert(targetIndex, tabItemSource);
                    tabItemSource.IsSelected = true;
                    tabControlTarget.HideTab();
                    tabControlSource.HideTab();
                }
            }
        }

        #endregion

        #region IDisposable

        private bool disposedValue;
        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MyTabControl.Items.Remove(this);
                    MyHeader.DragOver -= MyHeader_DragOver;
                    Drop -= TabItem_Drop;
                    PreviewMouseMove -= TabItem_PreviewMouseMove;
                    MyHeader.Label_TabTitle.SizeChanged -= TabTitle_SizeChanged;
                    Header = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
