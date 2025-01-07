using System.Collections.Generic;

namespace Explorip.Explorer.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemRegedit.xaml
    /// </summary>
    public partial class TabItemRegedit : TabItemExplorip
    {
        private readonly List<string> _historic = [];

        public TabItemRegedit()
        {
            InitializeComponent();
            InitializeExplorip();
            SetTitle("Regedit");
        }

        #region Search

        private void SearchText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // TODO
        }

        private void EditPath_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO
        }

        private void EditPath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // TODO
        }

        private void EditPath_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO
        }

        private void SearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO
        }

        #endregion

        #region Navigation

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO
        }

        private void PreviousButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO
        }

        #endregion
    }
}
