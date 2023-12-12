using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.WinAPI;

namespace Explorip.Helpers
{
    /// <summary>
    /// Logique d'interaction pour InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow()
        {
            InitializeComponent();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).Handle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
        }

        private void CommonInit(string title, string question)
        {
            Title = title;
            TxtQuestion.Text = question.Replace(@"\r\n", Environment.NewLine).Replace(@"\r", Environment.NewLine).Replace(@"\n", Environment.NewLine);
        }

        internal bool CheckValidPathName { get; set; }

        internal void ShowModal(string title, string question, string defaultValue = "")
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
