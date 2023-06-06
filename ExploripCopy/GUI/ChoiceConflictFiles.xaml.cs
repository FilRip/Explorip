using System.Windows;
using System.Windows.Input;

using ExploripCopy.Models;
using ExploripCopy.ViewModels;

namespace ExploripCopy.GUI
{
    /// <summary>
    /// Logique d'interaction pour ChoiceConflictFiles.xaml
    /// </summary>
    public partial class ChoiceConflictFiles : Window
    {
        public ChoiceConflictFiles()
        {
            InitializeComponent();
        }

        public ChoiceOnCollisionViewModel MyDataContext
        {
            get { return (ChoiceOnCollisionViewModel)DataContext; }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == ReplaceAll)
                MyDataContext.Choice = EChoiceFileOperation.ReplaceAll;
            else if (e.Source == IgnoreAll)
                MyDataContext.Choice = EChoiceFileOperation.KeepExisting;
            else if (e.Source == IgnoreSameDateSize)
                MyDataContext.Choice = EChoiceFileOperation.KeepMostRecent;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            MyDataContext.Choice = EChoiceFileOperation.None;
            Close();
        }

        private bool _startDrag;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_startDrag && WindowState != WindowState.Minimized && IsVisible && IsActive)
            {
                _startDrag = false;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startDrag = true;
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _startDrag = false;
        }
    }
}
