using PalletApp.Domain.Entities;
using PalletApp.Domain.Repositories;

namespace PalletApp.Application.Services;

public class PalletAppService : IPalletAppService
{
    private readonly IPalletRepository _palletRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IBinLocationRepository _binLocationRepository;
    private readonly IPrintRepository _printRepository;

    public PalletAppService(
        IPalletRepository palletRepository,
        IBatchRepository batchRepository,
        IBinLocationRepository binLocationRepository,
        IPrintRepository printRepository)
    {
        _palletRepository = palletRepository;
        _batchRepository = batchRepository;
        _binLocationRepository = binLocationRepository;
        _printRepository = printRepository;
    }

    public async Task<IEnumerable<BinLocationDto>> GetBinLocationsAsync()
    {
        var bins = await _binLocationRepository.GetAllActiveAsync();
        return bins.Select(b => new BinLocationDto(b.Id, b.LocationName, b.Description));
    }

    public async Task<IEnumerable<PalletDto>> GetPalletsAsync(int pageIndex, int pageSize, string? palletNo = null)
    {
        var pallets = await _palletRepository.GetAllAsync(pageIndex, pageSize, palletNo);
        // Note: Real mapping would use AutoMapper, doing manually for demo
        return pallets.Select(p => new PalletDto(
            p.Id, p.PalletNo, p.CreatedDate, p.CreatedBy, p.BinLocationId, null, p.Status, p.PalletBatches.Count
        ));
    }

    public async Task<PalletDto?> GetPalletByIdAsync(Guid id)
    {
        var p = await _palletRepository.GetByIdAsync(id);
        if (p == null) return null;
        return new PalletDto(p.Id, p.PalletNo, p.CreatedDate, p.CreatedBy, p.BinLocationId, null, p.Status, p.PalletBatches.Count);
    }

    public async Task<Guid> CreatePalletAsync(string createdBy, Guid binLocationId)
    {
        var pallet = new Pallet(createdBy, binLocationId);
        await _palletRepository.AddAsync(pallet);
        await _palletRepository.SaveChangesAsync();
        return pallet.Id;
    }

    public async Task ScanBatchToPalletAsync(Guid palletId, string batchId)
    {
        var pallet = await _palletRepository.GetByIdAsync(palletId) 
            ?? throw new Exception("Pallet not found.");

        var batch = await _batchRepository.GetByBatchIdAsync(batchId);
        if (batch == null)
        {
            // If batch doesn't exist in master, create it for demo
            batch = new Batch(batchId, "LINE-DEMO", DateTime.Now);
            await _batchRepository.AddAsync(batch);
        }
        else if (batch.Status == "Used")
        {
            throw new Exception($"Batch {batchId} is already assigned to a pallet.");
        }

        pallet.AddBatch(batch, "DemoUser");

        await _palletRepository.UpdateAsync(pallet);
        await _palletRepository.SaveChangesAsync();
    }

    public async Task GeneratePalletNumberAsync(Guid palletId)
    {
        var pallet = await _palletRepository.GetByIdAsync(palletId) 
            ?? throw new Exception("Pallet not found.");

        pallet.GeneratePalletNumber();

        await _palletRepository.UpdateAsync(pallet);
        await _palletRepository.SaveChangesAsync();
    }

    public async Task ConfirmPrintAsync(Guid palletId, string type, string @operator, string? printerName = null, string? notes = null)
    {
        var pallet = await _palletRepository.GetByIdAsync(palletId) 
            ?? throw new Exception("Pallet not found.");

        if (string.IsNullOrEmpty(pallet.PalletNo))
            throw new Exception("Generate Pallet Number first.");

        var printEvent = new PrintEvent(pallet.Id, type, @operator, printerName, notes);
        pallet.MarkAsPrinted();

        await _printRepository.AddPrintEventAsync(printEvent);
        await _palletRepository.UpdateAsync(pallet);
        await _palletRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<PalletBatchDto>> GetPalletBatchesAsync(Guid palletId)
    {
        var pallet = await _palletRepository.GetByIdAsync(palletId);
        if (pallet == null) return Enumerable.Empty<PalletBatchDto>();

        return pallet.PalletBatches.Select(pb => new PalletBatchDto(pb.SequenceNo, pb.Batch.BatchId, pb.AddedDate));
    }
}
