using System.Windows.Controls;

using ComputerInfo.ViewModels;

namespace ComputerInfo.Controls;

public partial class ComputerInfoControl : UserControl
{
    public ComputerInfoControl()
    {
        InitializeComponent();
    }

    public ComputerInfoViewModel MyDataContext
    {
        get { return (ComputerInfoViewModel)DataContext; }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (MyDataContext.IsPause)
            MyDataContext.Pause();
    }

    private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        MyDataContext.Dispose();
    }
}
