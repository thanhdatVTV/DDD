using System.Windows;
using PalletApp.Wpf.ViewModels;

namespace PalletApp.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(ScanBatchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}