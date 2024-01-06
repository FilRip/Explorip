using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

namespace ExploripSharedCopy.Controls
{
    /// <summary>
    /// Logique d'interaction pour InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow()
        {
            InitializeComponent();
            Owner = Application.Current.GetCurrentWindow();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
        }

        private void CommonInit(string title, string question)
        {
            Title = title;
            TxtQuestion.Text = question.Replace(@"\r\n", Environment.NewLine).Replace(@"\r", Environment.NewLine).Replace(@"\n", Environment.NewLine);
        }

        public bool CheckValidPathName { get; set; }

        public string Question
        {
            get { return TxtQuestion.Text; }
            set { TxtQuestion.Text = value; }
        }

        public string UserEdit
        {
            get { return TxtUserEdit.Text; }
            set { TxtUserEdit.Text = value; }
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

        public void ShowModal(string title, string question, string defaultValue = "")
        {
            CommonInit(title, question);
            TxtUserEdit.Text = defaultValue;
            ShowDialog();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValidPathName)
            {
                char[] invalidChar = ['\\', '/', ':', '*', '?', '<', '>', '|'];
                if (Array.Exists(invalidChar, c => TxtUserEdit.Text.Contains(c)))
                    return;
            }
            DialogResult = true;
            Close();
        }

        private void TxtUserEdit_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                BtnOk_Click(this, null);
            else if (e.Key == Key.Escape)
                BtnCancel_Click(this, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUserEdit.SelectAll();
        }
    }
}
