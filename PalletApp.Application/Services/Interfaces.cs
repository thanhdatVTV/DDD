namespace PalletApp.Application.Services;

public record BinLocationDto(Guid Id, string LocationName, string? Description);
public record PalletDto(Guid Id, string? PalletNo, DateTime CreatedDate, string CreatedBy, Guid? BinLocationId, string? BinLocationName, string Status, int BatchCount);
public record BatchDto(Guid Id, string BatchId, string? ProductionLine, DateTime? ProductionDate, string Status);
public record PalletBatchDto(int SequenceNo, string BatchId, DateTime AddedDate);

public interface IPalletAppService
{
    Task<IEnumerable<BinLocationDto>> GetBinLocationsAsync();
    Task<IEnumerable<PalletDto>> GetPalletsAsync(int pageIndex, int pageSize, string? palletNo = null);
    Task<PalletDto?> GetPalletByIdAsync(Guid id);
    Task<Guid> CreatePalletAsync(string createdBy, Guid binLocationId);
    Task ScanBatchToPalletAsync(Guid palletId, string batchId);
    Task GeneratePalletNumberAsync(Guid palletId);
    Task ConfirmPrintAsync(Guid palletId, string type, string @operator, string? printerName = null, string? notes = null);
    Task<IEnumerable<PalletBatchDto>> GetPalletBatchesAsync(Guid palletId);
}
