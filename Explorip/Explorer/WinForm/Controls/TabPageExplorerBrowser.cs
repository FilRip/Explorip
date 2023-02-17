using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Controls.WindowsForms;
using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.ComposantsWinForm
{
    public partial class TabPageExplorerBrowser : TabPage
    {
        private readonly ExplorerBrowser _explorerBrowser;
        private readonly Button _previousButton, _nextButton;
        private readonly SplitContainer _splitContainer;
        private readonly LinkLabel _pathLink;
        private readonly TextBox _txtEditPath;
        private readonly Stopwatch _stopWatch;
        private readonly TableLayoutPanel _panneauNavigation;
        private const int MinSizeNavigation = 25;
        private const int DEFAULT_BUTTON_SIZE = 16;

        public TabPageExplorerBrowser(ShellObject repertoireDemarrage)
        {
            InitializeComponent();
            SuspendLayout();
            DpiChangedAfterParent += TabPageExplorerBrowser_DpiChangedAfterParent;
            BorderStyle = BorderStyle.None;
            Margin = new Padding(0);
            _splitContainer = new SplitContainer();
            _splitContainer.SuspendLayout();
            _splitContainer.AutoScaleMode = AutoScaleMode.Dpi;
            _splitContainer.AutoScaleDimensions = new SizeF(96F, 96F);
            _splitContainer.Orientation = Orientation.Horizontal;
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Panel1.Margin = new Padding(0);
            _splitContainer.Panel1.Padding = new Padding(0);
            _splitContainer.Panel1MinSize = MinSizeNavigation;
            _splitContainer.SplitterDistance = MinSizeNavigation;
            _splitContainer.FixedPanel = FixedPanel.Panel1;
            _splitContainer.SplitterWidth = 1;
            _splitContainer.IsSplitterFixed = true;
            _splitContainer.Panel1.BackColor = Color.Transparent;
            _splitContainer.Panel1.ForeColor = Color.Transparent;
            Controls.Add(_splitContainer);

            _explorerBrowser = new ExplorerBrowser();
            _explorerBrowser.SuspendLayout();
            _splitContainer.Panel2.Controls.Add(_explorerBrowser);
            _explorerBrowser.Dock = DockStyle.Fill;
            _explorerBrowser.NavigationComplete += ExplorerBrowser_NavigationComplete;
            _explorerBrowser.NavigationPending += ExplorerBrowser_NavigationPending;
            _explorerBrowser.NavigationFailed += ExplorerBrowser_NavigationFailed;
            _explorerBrowser.NavigationOptions.PaneVisibility.Commands = PaneVisibilityState.Hide;
            _explorerBrowser.Navigate(repertoireDemarrage);

            _panneauNavigation = new TableLayoutPanel();
            _panneauNavigation.SuspendLayout();
            _panneauNavigation.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            _panneauNavigation.AutoSize = true;
            _panneauNavigation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _panneauNavigation.Margin = new Padding(0);
            _panneauNavigation.Padding = new Padding(0);
            _panneauNavigation.Dock = DockStyle.Fill;
            _panneauNavigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, DEFAULT_BUTTON_SIZE));
            _panneauNavigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, DEFAULT_BUTTON_SIZE));
            _panneauNavigation.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _panneauNavigation.ColumnCount = 3;
            _panneauNavigation.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _panneauNavigation.RowCount = 1;

            _splitContainer.Panel1.Controls.Add(_panneauNavigation);
            
            _previousButton = new Button();
            _previousButton.SuspendLayout();
            _previousButton.BackgroundImage = Properties.Resources.PreviousButton;
            _previousButton.BackgroundImageLayout = ImageLayout.Center;
            _previousButton.BackColor = Color.Transparent;
            _previousButton.Margin = new Padding(0);
            _previousButton.Padding = new Padding(0);
            _previousButton.FlatStyle = FlatStyle.Flat;
            _previousButton.ForeColor = Color.Transparent;
            _previousButton.Dock = DockStyle.Fill;
            _previousButton.FlatAppearance.BorderSize = 0;
            _previousButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _previousButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            _previousButton.Click += PreviousButton_Click;
            _previousButton.TabStop = false;

            _nextButton = new Button();
            _nextButton.SuspendLayout();
            _nextButton.BackgroundImage = Properties.Resources.NextButton;
            _nextButton.BackgroundImageLayout = ImageLayout.Center;
            _nextButton.BackColor = Color.Transparent;
            _nextButton.Margin = new Padding(0);
            _nextButton.Padding = new Padding(0);
            _nextButton.FlatStyle = FlatStyle.Flat;
            _nextButton.ForeColor = Color.Transparent;
            _nextButton.Dock = DockStyle.Fill;
            _nextButton.FlatAppearance.BorderSize = 0;
            _nextButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _nextButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            _nextButton.Click += NextButton_Click;
            _nextButton.TabStop = false;

            _panneauNavigation.Controls.Add(_previousButton, 0, 0);
            _panneauNavigation.Controls.Add(_nextButton, 1, 0);

            _pathLink = new LinkLabel();
            _pathLink.SuspendLayout();
            _pathLink.LinkColor = Color.Yellow;
            _pathLink.BackColor = Color.Transparent;
            _pathLink.TextAlign = ContentAlignment.MiddleLeft;
            _pathLink.ForeColor = Color.Yellow;
            _pathLink.AutoSize = true;
            _pathLink.Dock = DockStyle.Fill;
            _pathLink.LinkClicked += PathLink_LinkClicked;
            _pathLink.Click += PathLink_Click;
            _pathLink.UseCompatibleTextRendering = true;
            _panneauNavigation.Controls.Add(_pathLink, 2, 0);

            _txtEditPath = new TextBox();
            _txtEditPath.SuspendLayout();
            _txtEditPath.ForeColor = Color.Black;
            _txtEditPath.BackColor = Color.White;
            _txtEditPath.Visible = false;
            _txtEditPath.BorderStyle = BorderStyle.FixedSingle;
            _txtEditPath.Dock = DockStyle.Fill;
            _txtEditPath.KeyDown += TxtEditPath_KeyDown;
            _txtEditPath.MouseDoubleClick += TxtEditPath_MouseDoubleClick;
            _txtEditPath.LostFocus += TxtEditPath_LostFocus;
            _panneauNavigation.Controls.Add(_txtEditPath, 2, 0);

            _txtEditPath.ResumeLayout(false);
            _pathLink.ResumeLayout(false);
            _nextButton.ResumeLayout(false);
            _previousButton.ResumeLayout(false);
            _panneauNavigation.ResumeLayout(false);
            _explorerBrowser.ResumeLayout(false);
            _splitContainer.ResumeLayout(false);
            ResumeLayout(false);

            _stopWatch = new Stopwatch();

            RefreshNavigationHistory();
        }

        private void TabPageExplorerBrowser_DpiChangedAfterParent(object sender, EventArgs e)
        {
            _panneauNavigation.ColumnStyles[0].Width = (DEFAULT_BUTTON_SIZE * ((float)DeviceDpi / 96));
            _panneauNavigation.ColumnStyles[1].Width = (DEFAULT_BUTTON_SIZE * ((float)DeviceDpi / 96));
            _splitContainer.SplitterDistance = (int)(MinSizeNavigation * ((float)DeviceDpi / 96));
        }

        private void TxtEditPath_LostFocus(object sender, EventArgs e)
        {
            HideEditPath();
        }

        private void TxtEditPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                int pos = _txtEditPath.Text.LastIndexOf(@"\");
                _txtEditPath.SelectionStart = pos + 1;
                _txtEditPath.SelectionLength = _txtEditPath.Text.Length - pos;
                if (_txtEditPath.SelectionLength == 0)
                    _txtEditPath.SelectAll();
            }
            catch (Exception)
            {
                _txtEditPath.SelectAll();
            }
        }

        private void TxtEditPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                HideEditPath();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                ShellObject previousLocation = _explorerBrowser.NavigationLog.CurrentLocation;
                string nouvelEmplacement = _txtEditPath.Text;
                HideEditPath();
                _stopWatch.Restart();
                Task.Run(() =>
                {
                    try
                    {
                        _explorerBrowser.Navigate(ShellObject.FromParsingName(nouvelEmplacement));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Chemin non trouvé");
                        _explorerBrowser.Navigate(previousLocation);
                    }
                });
                _explorerBrowser.Focus();
            }
        }

        private void PathLink_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs args)
            {
                Size tailleTexte = TextRenderer.MeasureText(_pathLink.Text, _pathLink.Font);
                Rectangle rect = new(new Point(0, 0), tailleTexte);
                if (!rect.Contains(args.Location))
                {
                    _pathLink.Visible = false;
                    _txtEditPath.Visible = true;
                    _txtEditPath.Text = _pathLink.Text.Replace(@"\ ", @"\");
                    _txtEditPath.Focus();
                }
            }
        }

        private void PathLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _explorerBrowser.Navigate(ShellObject.FromParsingName(e.Link.Tag.ToString()));
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            _explorerBrowser.NavigateLogLocation(NavigationLogDirection.Forward);
            HideEditPath();
            RefreshNavigationHistory();
        }

        public void HideEditPath()
        {
            _txtEditPath.Visible = false;
            _pathLink.Visible = true;
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            _explorerBrowser.NavigateLogLocation(NavigationLogDirection.Backward);
            HideEditPath();
            RefreshNavigationHistory();
        }

        private void RefreshNavigationHistory()
        {
            _previousButton.Enabled = _explorerBrowser.NavigationLog.CanNavigateBackward;
            _previousButton.BackgroundImage = _previousButton.Enabled ? Themes.AutoTheme.ButtonPreviousEnabled : Themes.AutoTheme.ButtonPreviousDisabled;
            _nextButton.Enabled = _explorerBrowser.NavigationLog.CanNavigateForward;
            _nextButton.BackgroundImage = _nextButton.Enabled ? Themes.AutoTheme.ButtonNextEnabled : Themes.AutoTheme.ButtonNextDisabled;
        }

        private void ExplorerBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            _stopWatch.Stop();
        }

        private void ExplorerBrowser_NavigationPending(object sender, NavigationPendingEventArgs e)
        {
            if (!_stopWatch.IsRunning)
                _stopWatch.Restart();
            
        }

        private void RefreshSplitLinkLabel(string newPath)
        {
            _pathLink.Text = newPath.Replace(@"\", @"\ ");
            _pathLink.Links.Clear();
            int start = 0;
            StringBuilder partialPath = new();
            foreach (string path in _pathLink.Text.Split(@"\ "))
            {
                partialPath.Append(path + @"\");
                LinkLabel.Link lien = new()
                {
                    Start = start,
                    Length = path.Length,
                    Tag = partialPath.ToString(),
                };
                _pathLink.Links.Add(lien);
                start += path.Length + 2;
            }
        }

        private void ExplorerBrowser_NavigationComplete(object sender, NavigationCompleteEventArgs e)
        {
            _stopWatch.Stop();
            Text = e.NewLocation.Name;
            string fullPath = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
            try
            {
                RefreshSplitLinkLabel(fullPath);
            }
            catch (Exception)
            {
                _pathLink.Text = fullPath;
            }
            RefreshNavigationHistory();
            HideEditPath();
            _explorerBrowser.Focus();
        }

        public ExplorerBrowser ExplorerBrowser
        {
            get { return _explorerBrowser; }
        }

        public ShellObject RepertoireCourant
        {
            get { return _explorerBrowser.NavigationLog.CurrentLocation; }
        }
    }
}
