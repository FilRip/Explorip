using System.Drawing;
using System.Windows.Forms;

using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Controls.WindowsForms;
using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.ComposantsWinForm
{
    public partial class TabPageExplorerBrowser : TabPage
    {
        private readonly ExplorerBrowser _explorerBrowser;
        private readonly Button _previousButton, _nextButton;
        private readonly SplitContainer _splitContainer;

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
                Location = new Point(0, 0),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Transparent,
                ImageAlign = ContentAlignment.BottomRight,
            };
            _nextButton = new Button()
            {
                BackgroundImage = Properties.Resources.NextButton,
                BackgroundImageLayout = ImageLayout.Center,
                Location = new Point(18, 0),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                ForeColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.BottomRight,
            };
            _previousButton.FlatAppearance.BorderSize = 0;
            _previousButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _previousButton.FlatAppearance.MouseOverBackColor = Color.Gray;
            _nextButton.FlatAppearance.BorderSize = 0;
            _nextButton.FlatAppearance.MouseDownBackColor = WindowsSettings.GetWindowsAccentColor();
            _nextButton.FlatAppearance.MouseOverBackColor = Color.Gray;
            _splitContainer.Panel1.Controls.AddRange(new Control[] { _previousButton, _nextButton });
        }

        private void ExplorerBrowser_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            Text = e.NewLocation.Name;
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
