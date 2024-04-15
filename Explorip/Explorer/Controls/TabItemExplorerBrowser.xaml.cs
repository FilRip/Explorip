using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Explorer.ViewModels;
using Explorip.Explorer.Windows;

using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemExplorerBrowser.xaml
/// </summary>
public partial class TabItemExplorerBrowser : TabItemExplorip
{
    private readonly object _lockSearchText;

    private SearchCondition _searchQuery;
    private ShellSearchFolder _searchShell;
    private string _searchDirectory;

    public TabItemExplorerBrowser() : base()
    {
        InitializeComponent();
        InitializeExplorip();

        _lockSearchText = new object();

        ExplorerBrowser.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
        ExplorerBrowser.ExplorerBrowserControl.NavigationFailed += ExplorerBrowserControl_NavigationFailed;
        ExplorerBrowser.ExplorerBrowserControl.SelectionChanged += ExplorerBrowserControl_SelectionChanged;

        CurrentPath.MouseDown += CurrentPath_MouseDown;
    }

    public TabItemExplorerBrowserViewModel MyDataContext
    {
        get { return (TabItemExplorerBrowserViewModel)DataContext; }
    }

    public ShellObject CurrentDirectory
    {
        get { return ExplorerBrowser.NavigationLog.CurrentLocation; }
    }

    #region Navigation file explorer

    private void ExplorerBrowserControl_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        ExplorerBrowser.SetFocus();
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
                if (e.NewLocation.Name.Contains(Constants.Localization.SEARCH_RESULT.Replace("{0}", "")))
                    return;

                MyDataContext.ModeSearch = false;
                DisposeSearch();
                MyDataContext.ModeEdit = false;
                SetTitle(e.NewLocation.Name);

                string pathLink;
                bool splitPath = true;
                bool network = false;
                try
                {
                    if (e.NewLocation.IsFileSystemObject)
                        pathLink = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                    else
                        pathLink = e.NewLocation.Name;
                    network = pathLink.StartsWith($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}");
                }
                catch (Exception)
                {
                    pathLink = e.NewLocation.Name;
                    splitPath = false;
                }

                CurrentPath.Inlines?.Clear();
                if (splitPath)
                {
                    MyDataContext.EditPath = pathLink;
                    StringBuilder partialPath = new();
                    foreach (string path in pathLink.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (network && CurrentPath.Inlines.Count == 0)
                            partialPath.Append($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}");
                        partialPath.Append(path + Path.DirectorySeparatorChar);
                        Hyperlink lb = new()
                        {
                            Foreground = Brushes.Yellow,
                            NavigateUri = new Uri(partialPath.ToString()),
                        };
                        lb.RequestNavigate += Lb_RequestNavigate;
                        lb.Inlines.Add(path);
                        CurrentPath.Inlines.Add(lb);
                        CurrentPath.Inlines.Add($" {Path.DirectorySeparatorChar} ");
                    }
                }
                else
                {
                    CurrentPath.Inlines.Add(e.NewLocation.Name);
                }

                MyDataContext.AllowNavigatePrevious = ExplorerBrowser.NavigationLog.CanNavigateBackward;
                MyDataContext.AllowNavigateNext = ExplorerBrowser.NavigationLog.CanNavigateForward;
            }
        }
        catch (Exception)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }

    private void Lb_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        ExplorerBrowser.Navigate(ShellObject.FromParsingName(e.Uri.ToString()));
    }

    public void Navigation(string repertoire)
    {
        ExplorerBrowser.Navigate(ShellObject.FromParsingName(repertoire));
    }

    public void Navigation(ShellObject repertoire)
    {
        ExplorerBrowser.Navigate(repertoire);
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        ExplorerBrowser.NavigateLogLocation(NavigationLogDirection.Forward);
        ExplorerBrowser.SetFocus();
    }

    private void PreviousButton_Click(object sender, RoutedEventArgs e)
    {
        ExplorerBrowser.NavigateLogLocation(NavigationLogDirection.Backward);
        ExplorerBrowser.SetFocus();
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
                MyDataContext.EditPath = ExplorerBrowser.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                ExplorerBrowser.SetFocus();
            }
        }
        else if (e.Key is Key.Enter or Key.Return)
        {
            MyDataContext.ModeEdit = false;
            try
            {
                string nouvelEmplacement = Path.GetFullPath(Environment.ExpandEnvironmentVariables(EditPath.Text));
                Uri uri = new(nouvelEmplacement);
                if (!uri.IsFile || (!EditPath.Text.Contains(":") && !EditPath.Text.Contains("\\")))
                    throw new Exceptions.ExploripException("This is not a file:// protocol");

                ShellObject previousLocation = ExplorerBrowser.NavigationLog.CurrentLocation;

                Task.Run(() =>
                {
                    try
                    {
                        ExplorerBrowser.Navigate(ShellObject.FromParsingName(nouvelEmplacement));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Chemin non trouvé");
                        ExplorerBrowser.Navigate(previousLocation);
                    }
                });
            }
            catch (Exception)
            {
                Process.Start(EditPath.Text);
            }
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

    #region Search file/folder

    private void DisposeSearch()
    {
        _searchQuery?.Dispose();
        _searchShell?.Dispose();
        if (CurrentDirectory.Name.Contains(Constants.Localization.SEARCH_RESULT.Replace("{0}", "")))
        {
            CurrentDirectory.Dispose();
            Navigation(_searchDirectory);
        }
    }

    private void CreateSearch()
    {
        DisposeSearch();
        _searchQuery = SearchConditionFactory.CreateLeafCondition(SystemProperties.System.ItemName, SearchText.Text, SearchConditionOperation.ValueContains);
        _searchShell = new ShellSearchFolder(_searchQuery, _searchDirectory);
        _searchShell.SetDisplayName(string.Format(Constants.Localization.SEARCH_RESULT, _searchDirectory));
        ExplorerBrowser.ExplorerBrowserControl.ExplorerBrowserInterface.BrowseToObject(_searchShell.NativeShellItem, 0);
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        MyDataContext.ModeEdit = false;
        MyDataContext.ModeSearch = !MyDataContext.ModeSearch;
        SearchText.Text = "";
        if (MyDataContext.ModeSearch)
        {
            _searchDirectory = CurrentDirectory.ParsingName;
            SearchText.Focus();
        }
        else
            Navigation(_searchDirectory);
    }

    private void SearchText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            MyDataContext.ModeSearch = false;
            if (!string.IsNullOrWhiteSpace(_searchDirectory))
                Navigation(_searchDirectory);
        }
        else if ((e.Key == Key.Enter || e.Key == Key.Return) && !string.IsNullOrWhiteSpace(SearchText.Text))
        {
            lock (_lockSearchText)
            {
                CreateSearch();
            }
        }
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            DisposeSearch();
            ExplorerBrowser.Dispose();
        }
    }
}
