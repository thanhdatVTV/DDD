using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public enum PrintType
{
    A4,
    Label
}

public class PrintRecord : Entity
{
    public string PalletNo { get; private set; }
    public PrintType Type { get; private set; }
    public DateTime PrintTime { get; private set; } = DateTime.Now;
    public string Operator { get; private set; }

    public PrintRecord(string palletNo, PrintType type, string @operator)
    {
        PalletNo = palletNo;
        Type = type;
        Operator = @operator;
    }
}
