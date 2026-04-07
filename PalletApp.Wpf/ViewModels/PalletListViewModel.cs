using System.Collections.ObjectModel;
using PalletApp.Application.Services;
using PalletApp.Application.Services;

namespace PalletApp.Wpf.ViewModels;

public class PalletListViewModel : BaseViewModel
{
    private readonly IPalletAppService _palletService;
    private ObservableCollection<PalletDto> _pallets = new();
    private string? _palletNoFilter;

    public ObservableCollection<PalletDto> Pallets { get => _pallets; set { _pallets = value; OnPropertyChanged(); } }
    public string? PalletNoFilter { get => _palletNoFilter; set { _palletNoFilter = value; OnPropertyChanged(); } }

    public RelayCommand SearchCommand { get; }

    public PalletListViewModel(IPalletAppService palletService)
    {
        _palletService = palletService;
        SearchCommand = new RelayCommand(_ => LoadPallets());
        LoadPallets();
    }

    private async void LoadPallets()
    {
        var result = await _palletService.GetPalletsAsync(0, 20, PalletNoFilter);
        Pallets = new ObservableCollection<PalletDto>(result);
    }
}
