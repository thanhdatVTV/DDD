using System.Collections.ObjectModel;
using System.Windows;
using PalletApp.Application.Services;
using PalletApp.Application.Services;

namespace PalletApp.Wpf.ViewModels;

public class ScanBatchViewModel : BaseViewModel
{
    private readonly IPalletAppService _palletService;
    private Guid _currentPalletId;
    private ObservableCollection<BinLocationDto> _bins = new();
    private BinLocationDto? _selectedBin;
    private string? _batchIdInput;
    private ObservableCollection<PalletBatchDto> _scannedBatches = new();

    public ObservableCollection<BinLocationDto> Bins { get => _bins; set { _bins = value; OnPropertyChanged(); } }
    public BinLocationDto? SelectedBin { get => _selectedBin; set { _selectedBin = value; OnPropertyChanged(); } }
    public string? BatchIdInput { get => _batchIdInput; set { _batchIdInput = value; OnPropertyChanged(); } }
    public ObservableCollection<PalletBatchDto> ScannedBatches { get => _scannedBatches; set { _scannedBatches = value; OnPropertyChanged(); } }
    public int BatchCount => ScannedBatches.Count;

    public RelayCommand CreatePalletCommand { get; }
    public RelayCommand AddBatchCommand { get; }
    public RelayCommand GeneratePalletNoCommand { get; }

    public ScanBatchViewModel(IPalletAppService palletService)
    {
        _palletService = palletService;
        AddBatchCommand = new RelayCommand(_ => AddBatch(), _ => _currentPalletId != Guid.Empty);
        GeneratePalletNoCommand = new RelayCommand(_ => GeneratePalletNo(), _ => _currentPalletId != Guid.Empty);
        CreatePalletCommand = new RelayCommand(_ => CreatePallet(), _ => SelectedBin != null);
        
        LoadBins();
    }

    private async void LoadBins()
    {
        var bins = await _palletService.GetBinLocationsAsync();
        Bins = new ObservableCollection<BinLocationDto>(bins);
    }

    private async void CreatePallet()
    {
        if (SelectedBin == null) return;
        _currentPalletId = await _palletService.CreatePalletAsync("DemoUser", SelectedBin.Id);
        MessageBox.Show("New Pallet created in " + SelectedBin.LocationName);
    }

    private async void AddBatch()
    {
        if (string.IsNullOrWhiteSpace(BatchIdInput)) return;

        try
        {
            await _palletService.ScanBatchToPalletAsync(_currentPalletId, BatchIdInput);
            await LoadScannedBatches();
            BatchIdInput = string.Empty;
            
            if (BatchCount == 16)
            {
                MessageBox.Show("Pallet completed (16 batches). Ready to generate label.", "Info");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private async Task LoadScannedBatches()
    {
        var result = await _palletService.GetPalletBatchesAsync(_currentPalletId);
        ScannedBatches = new ObservableCollection<PalletBatchDto>(result);
        OnPropertyChanged(nameof(BatchCount));
    }

    private async void GeneratePalletNo()
    {
        if (BatchCount < 16)
        {
            var result = MessageBox.Show($"Pallet only has {BatchCount} batches. Generate anyway?", "Confirm", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
        }

        try
        {
            await _palletService.GeneratePalletNumberAsync(_currentPalletId);
            MessageBox.Show("Pallet Number Generated.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}
