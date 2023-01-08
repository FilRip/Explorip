using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private readonly Image _pbDisable, _pbEnable, _nbDisable, _nbEnable;
        private readonly TextBox _txtEditPath;
        private readonly Stopwatch _stopWatch;

        public TabPageExplorerBrowser(ShellObject repertoireDemarrage)
        {
            InitializeComponent();
            BorderStyle = BorderStyle.None;
            Margin = new Padding(0);
            _splitContainer = new SplitContainer();
            Controls.Add(_splitContainer);
            _splitContainer.AutoScaleMode = AutoScaleMode.Dpi;
            _splitContainer.Orientation = Orientation.Horizontal;
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.SplitterDistance = Font.Height * (DeviceDpi / 96);
            _splitContainer.FixedPanel = FixedPanel.Panel1;
            _splitContainer.IsSplitterFixed = true;
            _splitContainer.SplitterWidth = 1;
            _splitContainer.Panel1.BackColor = Color.Transparent;
            _splitContainer.Panel1.ForeColor = Color.Transparent;
            _explorerBrowser = new ExplorerBrowser();
            _splitContainer.Panel2.Controls.Add(_explorerBrowser);
            _explorerBrowser.Dock = DockStyle.Fill;
            _explorerBrowser.NavigationComplete += ExplorerBrowser_NavigationComplete;
            _explorerBrowser.NavigationPending += ExplorerBrowser_NavigationPending;
            _explorerBrowser.NavigationFailed += ExplorerBrowser_NavigationFailed;
            _explorerBrowser.Navigate(repertoireDemarrage);

            _previousButton = new Button()
            {
                BackgroundImage = Properties.Resources.PreviousButton,
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(0, 3 * (DeviceDpi / 96)),
                Size = new Size(16 * (DeviceDpi / 96), 16 * (DeviceDpi / 96)),
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

            _nextButton = new Button()
            {
                BackgroundImage = Properties.Resources.NextButton,
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(_previousButton.Location.X + _previousButton.Size.Width + 2 * (DeviceDpi / 96), 3 * (DeviceDpi / 96)),
                Size = new Size(16 * (DeviceDpi / 96), 16 * (DeviceDpi / 96)),
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Transparent,
            };
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
                Location = new Point(_nextButton.Location.X + _nextButton.Size.Width + 3, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                LinkColor = Color.Yellow,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Yellow,
            };
            _pathLink.Height *= (DeviceDpi / 96);
            _pathLink.Font = new Font(_pathLink.Font.FontFamily, _pathLink.Height / 4);
            _pathLink.Width = _splitContainer.Panel1.Width - _pathLink.Location.X;
            _pathLink.LinkClicked += PathLink_LinkClicked;
            _pathLink.Click += PathLink_Click;
            _splitContainer.Panel1.Controls.Add(_pathLink);

            _txtEditPath = new TextBox()
            {
                Location = new Point(_nextButton.Location.X + _nextButton.Size.Width + 3, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                ForeColor = Color.Black,
                BackColor = Color.White,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
            };
            _txtEditPath.Height *= (DeviceDpi / 96);
            _txtEditPath.Font = _pathLink.Font;
            _txtEditPath.Width = _splitContainer.Panel1.Width - _txtEditPath.Location.X;
            _txtEditPath.KeyDown += TxtEditPath_KeyDown;
            _txtEditPath.MouseDoubleClick += TxtEditPath_MouseDoubleClick;
            _txtEditPath.LostFocus += TxtEditPath_LostFocus;
            _splitContainer.Panel1.Controls.Add(_txtEditPath);

            _splitContainer.SplitterDistance = _txtEditPath.Location.Y + _txtEditPath.Size.Height + 4;

            _stopWatch = new Stopwatch();

            RefreshNavigationHistory();
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
            _previousButton.BackgroundImage = _previousButton.Enabled ? _pbEnable : _pbDisable;
            _nextButton.Enabled = _explorerBrowser.NavigationLog.CanNavigateForward;
            _nextButton.BackgroundImage = _nextButton.Enabled ? _nbEnable : _nbDisable;
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

        private void ExplorerBrowser_NavigationComplete(object sender, NavigationCompleteEventArgs e)
        {
            _stopWatch.Stop();
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
