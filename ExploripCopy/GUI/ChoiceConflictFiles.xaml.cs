using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
