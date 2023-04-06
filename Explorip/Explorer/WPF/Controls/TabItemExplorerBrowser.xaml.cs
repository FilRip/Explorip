using Explorip.Explorer.WPF.ViewModels;
using Explorip.Explorer.WPF.Windows;
using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemExplorerBrowser.xaml
    /// </summary>
    public partial class TabItemExplorerBrowser : TabItem, IDisposable
    {
        private string _searchDirectory;
        private string _lastFile;
        private bool disposedValue;
        private readonly object _lockSearchText;
        private bool _allowSearch;

        public TabItemExplorerBrowser()
        {
            InitializeComponent();

            _lockSearchText = new object();
            _allowSearch = true;

            ExplorerBrowser.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
            ExplorerBrowser.ExplorerBrowserControl.NavigationFailed += ExplorerBrowserControl_NavigationFailed;
            ExplorerBrowser.ExplorerBrowserControl.SelectionChanged += ExplorerBrowserControl_SelectionChanged;

            // Support close button in header
            HeaderWithCloseButton closableTabHeader = new();
            Header = closableTabHeader;
            MyHeader.DragOver += MyHeader_DragOver;

            closableTabHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;

            CurrentPath.MouseDown += CurrentPath_MouseDown;
        }

        private void MyHeader_DragOver(object sender, DragEventArgs e)
        {
            TabItemExplorerBrowser tab = (TabItemExplorerBrowser)((HeaderWithCloseButton)e.Source).Parent;
            if (MyTabControl.SelectedItem != tab &&
                e.Data.GetData("FileDrop") != null)
            {
                MyTabControl.SelectedItem = tab;
            }
        }

        public HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        public TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)Parent; }
        }

        /// <summary>
        /// Property - Set the Title of the Tab
        /// </summary>
        public void SetTitle(string newTitle)
        {
            MyHeader.Label_TabTitle.Content = newTitle;
        }

        public TabItemExplorerBrowserViewModel MyDataContext
        {
            get { return (TabItemExplorerBrowserViewModel)DataContext; }
        }

        public ShellObject CurrentDirectory
        {
            get { return ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation; }
        }

        #region Navigation file explorer

        private void ExplorerBrowserControl_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.Focus();
            _allowSearch = true;
        }

        private void ExplorerBrowserControl_SelectionChanged(object sender, EventArgs e)
        {
            if (MyTabControl == ((WpfExplorerBrowser)Window.GetWindow(MyTabControl)).LeftTab)
                ((WpfExplorerBrowser)Window.GetWindow(MyTabControl)).MyDataContext.SelectionLeft = ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0;
            else
                ((WpfExplorerBrowser)Window.GetWindow(MyTabControl)).MyDataContext.SelectionRight = ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0;
        }

        private void ExplorerBrowserControl_NavigationComplete(object sender, NavigationCompleteEventArgs e)
        {
            try
            {
                lock (_lockSearchText)
                {
                    MyDataContext.ModeEdit = false;
                    if (!SearchText.IsFocused || !e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath).StartsWith(Environment.SpecialFolder.UserProfile.Repertoire() + $"\\Searches"))
                    {
                        SetTitle(e.NewLocation.Name);
                        MyDataContext.ModeSearch = false;
                        _searchDirectory = null;
                        if (!string.IsNullOrWhiteSpace(_lastFile))
                        {
                            File.Delete(_lastFile);
                            _lastFile = null;
                        }
                        SearchText.Text = "";
                    }

                    string pathLink;
                    bool splitPath = true;
                    try
                    {
                        pathLink = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                    }
                    catch (Exception)
                    {
                        pathLink = e.NewLocation.Name;
                        splitPath = false;
                    }

                    if (!MyDataContext.ModeSearch)
                    {
                        CurrentPath.Inlines?.Clear();
                        if (splitPath)
                        {
                            MyDataContext.EditPath = pathLink;
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
                                CurrentPath.Inlines.Add(" \\ ");
                            }
                        }
                        else
                        {
                            CurrentPath.Inlines.Add(e.NewLocation.Name);
                        }
                    }

                    MyDataContext.AllowNavigatePrevious = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CanNavigateBackward;
                    MyDataContext.AllowNavigateNext = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CanNavigateForward;
                }
            }
            catch (Exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
            finally
            {
                _allowSearch = true;
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

        public void Navigation(ShellObject repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(repertoire);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.NavigateLogLocation(NavigationLogDirection.Forward);
            ExplorerBrowser.ExplorerBrowserControl.Focus();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.NavigateLogLocation(NavigationLogDirection.Backward);
            ExplorerBrowser.ExplorerBrowserControl.Focus();
        }

        #endregion

        #region Support Close button on Header

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            MyHeader.ButtonClose.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                MyHeader.ButtonClose.Visibility = Visibility.Hidden;
            }
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonClose.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 0, 0);
        }

        #endregion

        #region Edit path manually

        private void EditPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (MyDataContext.ShowSuggestions)
                {
                    MyDataContext.ShowSuggestions = false;
                }
                else
                {
                    MyDataContext.ModeEdit = false;
                    MyDataContext.EditPath = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                    ExplorerBrowser.ExplorerBrowserControl.Focus();
                }
            }
            else if (e.Key is Key.Enter or Key.Return)
            {
                ShellObject previousLocation = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation;
                string nouvelEmplacement = EditPath.Text;
                MyDataContext.ModeEdit = false;
                Task.Run(() =>
                {
                    try
                    {
                        ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(nouvelEmplacement));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Chemin non trouvé");
                        ExplorerBrowser.ExplorerBrowserControl.Navigate(previousLocation);
                    }
                });
            }
        }

        private void CurrentPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyDataContext.ModeEdit = true;
            MyDataContext.ShowSuggestions = false;
            EditPath.ApplyTemplate();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TextBox comboTextBoxChild = EditPath.Template.FindName("PART_EditableTextBox", EditPath) as TextBox;
                comboTextBoxChild.Focus();
                comboTextBoxChild.CaretIndex = comboTextBoxChild.Text.Length;
                comboTextBoxChild.SelectAll();
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void EditPath_LostFocus(object sender, RoutedEventArgs e)
        {
            MyDataContext.ModeEdit = false;
        }

        private void EditPath_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(EditPath, e.GetPosition(EditPath));
            if (result?.VisualHit is Border)
            {
                TextBox comboTextBoxChild = EditPath.Template.FindName("PART_EditableTextBox", EditPath) as TextBox;
                comboTextBoxChild.CaretIndex = comboTextBoxChild.Text.Length;
                if (e.ClickCount == 2)
                    try
                    {
                        comboTextBoxChild.SelectionStart = EditPath.Text.LastIndexOf(@"\") + 1;
                        comboTextBoxChild.SelectionLength = EditPath.Text.Length - comboTextBoxChild.SelectionStart;
                    }
                    catch (Exception) { /* Ignoring errors */ }
                else if (e.ClickCount == 3)
                    comboTextBoxChild.SelectAll();
                e.Handled = true;
            }
        }

        #endregion

        #region drag's drop tabitem in tabcontrol

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is not TabItem tabItem)
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
            TabItemExplorerBrowser tabItemTarget = null;
            if (e.Source is TabItemExplorerBrowser browser)
                tabItemTarget = browser;
            else if (e.Source is HeaderWithCloseButton entete && entete.Parent is TabItemExplorerBrowser browser2)
                tabItemTarget = browser2;

            if (tabItemTarget != null &&
                e.Data.GetData(typeof(TabItemExplorerBrowser)) is TabItemExplorerBrowser tabItemSource &&
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
                else
                {
                    if (tabControlSource.Items.Count > 1 || tabControlSource.AllowCloseLastTab)
                        tabControlSource.Items.Remove(tabItemSource);
                    else
                    {
                        ShellObject repertoire = tabItemSource.ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation;
                        tabItemSource = new TabItemExplorerBrowser();
                        tabItemSource.Navigation(repertoire);
                    }
                    tabControlTarget.Items.Insert(targetIndex, tabItemSource);
                    tabItemSource.IsSelected = true;
                    tabControlTarget.HideTab();
                    tabControlSource.HideTab();
                }
            }
        }

        #endregion

        #region Search file/folder

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MyDataContext.ModeEdit = false;
            MyDataContext.ModeSearch = !MyDataContext.ModeSearch;
            if (MyDataContext.ModeSearch)
                SearchText.Focus();
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                lock (_lockSearchText)
                {
                    if (!_allowSearch)
                        return;

                    if (MyDataContext.ModeSearch && ExplorerBrowser?.ExplorerBrowserControl?.NavigationLog?.CurrentLocation != null)
                    {
                        _allowSearch = false;
                        string previousFile = null;

                        if (string.IsNullOrWhiteSpace(_searchDirectory))
                            _searchDirectory = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                        else
                            previousFile = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);

                        string dir = _searchDirectory;
                        // https://learn.microsoft.com/en-us/windows/win32/search/-search-savedsearchfileformat
                        string xmlContent = $"<?xml version=\"1.0\"?><persistedQuery version=\"1.0\"><query><conditions><condition type=\"leafCondition\" property=\"System.Generic.String\" operator=\"wordmatch\" propertyType=\"string\" value=\"{SearchText.Text}\" localeName=\"fr-FR\"/></conditions><kindList><kind name=\"item\"/></kindList><scope><include path=\"::{{20D04FE0-3AEA-1069-A2D8-08002B30309D}}\\{dir}\"/></scope></query></persistedQuery>";
                        string filename = Environment.SpecialFolder.UserProfile.Repertoire() + $"\\Searches\\{DateTime.Now.Year:0000}{DateTime.Now.Month:00}{DateTime.Now.Hour:00}{DateTime.Now.Minute:00}{DateTime.Now.Second:00}{DateTime.Now.Millisecond:000}.search-ms";
                        File.AppendAllText(filename, xmlContent);

                        Navigation(filename);

                        _lastFile = filename;

                        if (!string.IsNullOrWhiteSpace(previousFile))
                        {
                            File.Delete(previousFile);
                            ExplorerBrowser.NavigationLog.RemoveAt(ExplorerBrowser.NavigationLog.Count - 1);
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
        }

        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MyDataContext.ModeSearch = false;
                if (!string.IsNullOrWhiteSpace(_searchDirectory))
                    Navigation(_searchDirectory);
            }
        }

        #endregion

        private void TabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_lastFile))
                File.Delete(_lastFile);
        }

        #region IDispose interface

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
                    ExplorerBrowser.Dispose();
                    TabItem_Unloaded(null, null);
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
