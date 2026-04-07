using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public class PrintEvent : Entity
{
    public Guid PalletId { get; private set; }
    public string PrintType { get; private set; }
    public string PrintedBy { get; private set; }
    public DateTime PrintedDate { get; private set; } = DateTime.Now;
    public string? PrinterName { get; private set; }
    public bool IsSuccess { get; private set; } = true;
    public string? Notes { get; private set; }

    public PrintEvent(Guid palletId, string printType, string printedBy, string? printerName = null, string? notes = null)
    {
        PalletId = palletId;
        PrintType = printType;
        PrintedBy = printedBy;
        PrinterName = printerName;
        Notes = notes;
    }
}
