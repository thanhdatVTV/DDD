using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public class Batch : Entity
{
    public string BatchId { get; private set; }
    public string? ProductionLine { get; private set; }
    public DateTime? ProductionDate { get; private set; }
    public string Status { get; private set; } = "Available";
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    public Batch(string batchId, string? productionLine = null, DateTime? productionDate = null)
    {
        if (string.IsNullOrWhiteSpace(batchId)) throw new ArgumentException("BatchId is required.");
        BatchId = batchId;
        ProductionLine = productionLine;
        ProductionDate = productionDate;
    }

    public void MarkAsUsed() => Status = "Used";
}
