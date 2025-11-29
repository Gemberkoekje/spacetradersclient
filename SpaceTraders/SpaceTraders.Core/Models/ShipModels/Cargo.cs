using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Cargo
{
    required public int Capacity { get; init; }
    required public int Units { get; init; }
    required public ImmutableHashSet<CargoItem> Inventory { get; init; }
}
