using PalletApp.Domain.Entities;

namespace PalletApp.Domain.Repositories;

public interface IPalletRepository
{
    Task<Pallet?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pallet>> GetAllAsync(int pageIndex, int pageSize, string? palletNo = null);
    Task AddAsync(Pallet pallet);
    Task UpdateAsync(Pallet pallet);
    Task SaveChangesAsync();
}

public interface IBatchRepository
{
    Task<Batch?> GetByBatchIdAsync(string batchId);
    Task AddAsync(Batch batch);
    Task<bool> IsBatchUsedAsync(string batchId);
}

public interface IBinLocationRepository
{
    Task<IEnumerable<BinLocation>> GetAllActiveAsync();
    Task<BinLocation?> GetByIdAsync(Guid id);
    Task AddAsync(BinLocation binLocation);
}

public interface IPrintRepository
{
    Task AddPrintEventAsync(PrintEvent printEvent);
    Task SaveChangesAsync();
}
