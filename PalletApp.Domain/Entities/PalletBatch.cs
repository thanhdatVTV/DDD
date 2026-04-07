using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public class PalletBatch : Entity
{
    public Guid PalletId { get; private set; }
    public Guid BatchId { get; private set; }
    public int SequenceNo { get; private set; }
    public DateTime AddedDate { get; private set; } = DateTime.Now;
    public string AddedBy { get; private set; }

    // Navigation property for EF (though in DDD we might refer by ID, for complex schema we keep navigation if needed or just IDs)
    public Batch Batch { get; set; } = null!;

    public PalletBatch(Guid palletId, Guid batchId, int sequenceNo, string addedBy)
    {
        PalletId = palletId;
        BatchId = batchId;
        SequenceNo = sequenceNo;
        AddedBy = addedBy;
    }
}
