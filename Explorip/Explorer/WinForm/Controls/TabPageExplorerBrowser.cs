using System;
using System.Drawing;
using System.IO;
using System.Text;
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
        private readonly Image _pbDisable, _pbEnable, _nbDisable, _nbEnable;
        private readonly TextBox _txtEditPath;

        public TabPageExplorerBrowser(ShellObject repertoireDemarrage)
        {
            InitializeComponent();
            BorderStyle = BorderStyle.None;
            Margin = new Padding(0);
            _splitContainer = new SplitContainer();
            Controls.Add(_splitContainer);
            _splitContainer.Orientation = Orientation.Horizontal;
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.SplitterDistance = Font.Height;
            _splitContainer.FixedPanel = FixedPanel.Panel1;
            _splitContainer.IsSplitterFixed = true;
            _splitContainer.SplitterWidth = 1;
            _splitContainer.Panel1.BackColor = Color.Transparent;
            _splitContainer.Panel1.ForeColor = Color.Transparent;
            _explorerBrowser = new ExplorerBrowser();
            _splitContainer.Panel2.Controls.Add(_explorerBrowser);
            _explorerBrowser.Dock = DockStyle.Fill;
            _explorerBrowser.NavigationComplete += ExplorerBrowser_NavigationComplete;
            _explorerBrowser.Navigate(repertoireDemarrage);

            _previousButton = new Button()
            {
                BackgroundImage = Properties.Resources.PreviousButton,
                BackgroundImageLayout = ImageLayout.Center,
                Location = new Point(0, 3),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Transparent,
            };
            _nextButton = new Button()
            {
                BackgroundImage = Properties.Resources.NextButton,
                BackgroundImageLayout = ImageLayout.Center,
                Location = new Point(18, 3),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Transparent,
            };
            _previousButton.FlatAppearance.BorderSize = 0;
            _previousButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _previousButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            _previousButton.Click += PreviousButton_Click;
            _previousButton.TabStop = false;
            _nextButton.FlatAppearance.BorderSize = 0;
            _nextButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _nextButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            _nextButton.Click += NextButton_Click;
            _nextButton.TabStop = false;
            _splitContainer.Panel1.Controls.AddRange(new Control[] { _previousButton, _nextButton });

            _pbDisable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, Color.LightGray);
            _pbEnable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());

            _nbDisable = Properties.Resources.NextButton.ChangeCouleur(Color.White, Color.LightGray);
            _nbEnable = Properties.Resources.NextButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());

            _pathLink = new LinkLabel()
            {
                Location = new Point(36, 0),
                Width = _splitContainer.Panel1.Width - 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                LinkColor = Color.Yellow,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Yellow,
            };
            _pathLink.LinkClicked += PathLink_LinkClicked;
            _pathLink.Click += PathLink_Click;
            _splitContainer.Panel1.Controls.Add(_pathLink);

            _txtEditPath = new TextBox()
            {
                Location = new Point(36, 0),
                Width = _splitContainer.Panel1.Width - 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                ForeColor = Color.Black,
                BackColor = Color.White,
                Visible = false,
            };
            _txtEditPath.KeyDown += TxtEditPath_KeyDown;
            _splitContainer.Panel1.Controls.Add(_txtEditPath);

            RefreshNavigationHistory();
        }

        private void TxtEditPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _txtEditPath.Visible = false;
                _pathLink.Visible = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                string nouvelEmplacement = _txtEditPath.Text;
                if (new DirectoryInfo(nouvelEmplacement).Exists)
                {
                    _txtEditPath.Visible = false;
                    _pathLink.Visible = true;
                    _explorerBrowser.Navigate(ShellObject.FromParsingName(nouvelEmplacement));
                }
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
                    _txtEditPath.SelectAll();
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
            RefreshNavigationHistory();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            _explorerBrowser.NavigateLogLocation(NavigationLogDirection.Backward);
            RefreshNavigationHistory();
        }

        private void RefreshNavigationHistory()
        {
            _previousButton.Enabled = _explorerBrowser.NavigationLog.CanNavigateBackward;
            _previousButton.BackgroundImage = _previousButton.Enabled ? _pbEnable : _pbDisable;
            _nextButton.Enabled = _explorerBrowser.NavigationLog.CanNavigateForward;
            _nextButton.BackgroundImage = _nextButton.Enabled ? _nbEnable : _nbDisable;
        }

        private void ExplorerBrowser_NavigationComplete(object sender, NavigationCompleteEventArgs e)
        {
            Text = e.NewLocation.Name;
            string fullPath = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
            try
            {
                _pathLink.Text = fullPath.Replace(@"\", @"\ ");
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
            catch (Exception)
            {
                _pathLink.Text = fullPath;
            }
            RefreshNavigationHistory();
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
