using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Explorip.Explorer.WPF.Controls
{
    public abstract class TabItemExplorip : TabItem, IDisposable
    {
        protected void InitializeExplorip()
        {
            HeaderWithCloseButton closableTabHeader = new();
            Header = closableTabHeader;
            MyHeader.DragOver += MyHeader_DragOver;
            closableTabHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;
        }

        private void MyHeader_DragOver(object sender, DragEventArgs e)
        {
            TabItemExplorip tab = (TabItemExplorip)((HeaderWithCloseButton)e.Source).Parent;
            if (MyTabControl.SelectedItem != tab &&
                e.Data.GetData("FileDrop") != null)
            {
                MyTabControl.SelectedItem = tab;
            }
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonClose.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 0, 0);
        }

        /// <summary>
        /// Property - Set the Title of the Tab
        /// </summary>
        public void SetTitle(string newTitle)
        {
            if (MyHeader != null)
                MyHeader.Label_TabTitle.Content = newTitle;
        }

        protected HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        protected TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)Parent; }
        }

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
    }
}
