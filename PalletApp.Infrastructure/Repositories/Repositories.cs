using Microsoft.EntityFrameworkCore;
using PalletApp.Domain.Entities;
using PalletApp.Domain.Repositories;

namespace PalletApp.Infrastructure.Repositories;

public class PalletRepository : IPalletRepository
{
    private readonly PalletAppDbContext _context;

    public PalletRepository(PalletAppDbContext context)
    {
        _context = context;
    }

    public async Task<Pallet?> GetByIdAsync(Guid id)
    {
        return await _context.Pallets
            .Include(p => p.PalletBatches)
                .ThenInclude(pb => pb.Batch)
            .Include(p => p.PrintEvents)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pallet>> GetAllAsync(int pageIndex, int pageSize, string? palletNo = null)
    {
        var query = _context.Pallets
            .AsNoTracking()
            .Include(p => p.PalletBatches)
            .AsQueryable();

        if (!string.IsNullOrEmpty(palletNo))
            query = query.Where(p => p.PalletNo.Contains(palletNo));

        return await query
            .OrderByDescending(p => p.CreatedDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Pallet pallet)
    {
        await _context.Pallets.AddAsync(pallet);
    }

    public async Task UpdateAsync(Pallet pallet)
    {
        _context.Entry(pallet).State = EntityState.Modified;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class BatchRepository : IBatchRepository
{
    private readonly PalletAppDbContext _context;

    public BatchRepository(PalletAppDbContext context)
    {
        _context = context;
    }

    public async Task<Batch?> GetByBatchIdAsync(string batchId)
    {
        return await _context.Batches.FirstOrDefaultAsync(b => b.BatchId == batchId);
    }

    public async Task AddAsync(Batch batch)
    {
        await _context.Batches.AddAsync(batch);
    }

    public async Task<bool> IsBatchUsedAsync(string batchId)
    {
        return await _context.Batches.AnyAsync(b => b.BatchId == batchId && b.Status == "Used");
    }
}

public class BinLocationRepository : IBinLocationRepository
{
    private readonly PalletAppDbContext _context;

    public BinLocationRepository(PalletAppDbContext context) => _context = context;

    public async Task<IEnumerable<BinLocation>> GetAllActiveAsync() => 
        await _context.BinLocations.Where(b => b.IsActive).ToListAsync();

    public async Task<BinLocation?> GetByIdAsync(Guid id) => 
        await _context.BinLocations.FindAsync(id);

    public async Task AddAsync(BinLocation binLocation) => 
        await _context.BinLocations.AddAsync(binLocation);
}

public class PrintRepository : IPrintRepository
{
    private readonly PalletAppDbContext _context;

    public PrintRepository(PalletAppDbContext context) => _context = context;

    public async Task AddPrintEventAsync(PrintEvent printEvent) => 
        await _context.PrintEvents.AddAsync(printEvent);

    public async Task SaveChangesAsync() => 
        await _context.SaveChangesAsync();
}
