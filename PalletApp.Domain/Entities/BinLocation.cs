using PalletApp.Domain.Common;

namespace PalletApp.Domain.Entities;

public class BinLocation : Entity
{
    public string LocationName { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Pallet> _pallets = new();
    public IReadOnlyCollection<Pallet> Pallets => _pallets.AsReadOnly();

    public BinLocation(string locationName, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(locationName)) throw new ArgumentException("LocationName is required.");
        LocationName = locationName;
        Description = description;
    }
}
