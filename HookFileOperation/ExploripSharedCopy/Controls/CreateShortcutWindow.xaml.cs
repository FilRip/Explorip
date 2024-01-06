using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using Microsoft.Win32;

namespace ExploripSharedCopy.Controls
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class CreateShortcutWindow : Window
    {
        public CreateShortcutWindow()
        {
            InitializeComponent();
            Owner = Application.Current.GetCurrentWindow();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
        }

        public string Target
        {
            get { return TxtTarget.Text; }
            set { TxtTarget.Text = value; }
        }

        public string ShortcutName
        {
            get { return TxtName.Text; }
            set { TxtName.Text = value; }
        }

        public void SetQuestions(string q1, string q2)
        {
            QuestionOne.Text = q1;
            QuestionTwo.Text = q2;
        }

        public void SetOk(string text, ImageSource image)
        {
            TextOk.Text = text;
            ImageOk.Source = image;
        }

        public void SetCancel(string text, ImageSource image)
        {
            TextCancel.Text = text;
            ImageCancel.Source = image;
        }

        public void SetBrowse(string text)
        {
            BtnBrowseTarget.Content = text;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                BtnCancel_Click(sender, e);
        }

        private void BtnBrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                TxtTarget.Text = dialog.FileName;
                TxtName.Text = Path.GetFileName(dialog.FileName);
            }
        }
    }
}
