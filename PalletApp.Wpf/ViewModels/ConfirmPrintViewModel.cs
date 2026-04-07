using System.Windows;
using PalletApp.Application.Services;
using PalletApp.Application.Services;

namespace PalletApp.Wpf.ViewModels;

public class ConfirmPrintViewModel : BaseViewModel
{
    private readonly IPalletAppService _palletService;
    private PalletDto? _currentPallet;

    public PalletDto? CurrentPallet { get => _currentPallet; set { _currentPallet = value; OnPropertyChanged(); } }

    public RelayCommand PrintA4Command { get; }
    public RelayCommand PrintLabelCommand { get; }

    public ConfirmPrintViewModel(IPalletAppService palletService, PalletDto pallet)
    {
        _palletService = palletService;
        _currentPallet = pallet;
        PrintA4Command = new RelayCommand(_ => Print("A4"));
        PrintLabelCommand = new RelayCommand(_ => Print("Label"));
    }

    private async void Print(string type)
    {
        if (CurrentPallet == null) return;
        try
        {
            await _palletService.ConfirmPrintAsync(CurrentPallet.Id, type, "DemoUser");
            MessageBox.Show($"{type} printed and logged successfully for Pallet {CurrentPallet.PalletNo}", "Print Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}
