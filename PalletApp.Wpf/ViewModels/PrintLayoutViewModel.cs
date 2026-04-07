using PalletApp.Application.Services;

namespace PalletApp.Wpf.ViewModels;

public class PrintLayoutViewModel : BaseViewModel
{
    private PalletDto? _pallet;
    public PalletDto? Pallet { get => _pallet; set { _pallet = value; OnPropertyChanged(); } }

    public PrintLayoutViewModel(PalletDto pallet)
    {
        _pallet = pallet;
    }
}
