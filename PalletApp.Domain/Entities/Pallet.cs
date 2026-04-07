using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public class Pallet : AggregateRoot
{
    private readonly List<PalletBatch> _palletBatches = new();
    private readonly List<PrintEvent> _printEvents = new();

    public string? PalletNo { get; private set; }
    public Guid? BinLocationId { get; private set; }
    public string Status { get; private set; } = "Draft";
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public bool IsDeleted { get; private set; } = false;

    public IReadOnlyCollection<PalletBatch> PalletBatches => _palletBatches.AsReadOnly();
    public IReadOnlyCollection<PrintEvent> PrintEvents => _printEvents.AsReadOnly();

    public Pallet(string createdBy, Guid? binLocationId = null)
    {
        CreatedBy = createdBy;
        BinLocationId = binLocationId;
    }

    public void AddBatch(Batch batch, string addedBy)
    {
        if (Status == "Completed" || Status == "Printed")
            throw new InvalidOperationException("Cannot add batch to a completed or printed pallet.");

        if (_palletBatches.Count >= 16)
            throw new InvalidOperationException("Pallet is full (16 batches max).");

        if (_palletBatches.Any(pb => pb.BatchId == batch.Id))
            throw new InvalidOperationException("Batch already exists in this pallet.");

        var sequenceNo = _palletBatches.Count + 1;
        var palletBatch = new PalletBatch(Id, batch.Id, sequenceNo, addedBy);
        _palletBatches.Add(palletBatch);
        
        batch.MarkAsUsed();
    }

    public void GeneratePalletNumber()
    {
        if (string.IsNullOrEmpty(PalletNo))
        {
            PalletNo = $"P{DateTime.Now:yyMMdd}{Guid.NewGuid().ToString()[..4].ToUpper()}";
            Status = "Completed";
        }
    }

    public void MarkAsPrinted() => Status = "Printed";

    public void SetBinLocation(Guid binLocationId) => BinLocationId = binLocationId;
}
