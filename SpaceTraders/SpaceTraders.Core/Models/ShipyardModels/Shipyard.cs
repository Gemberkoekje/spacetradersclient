using SpaceTraders.Core.Enums;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipyardModels;

public sealed record Shipyard
{
    required public string Symbol { get; init; }
    required public ImmutableHashSet<ShipType> ShipTypes { get; init; }
    required public ImmutableHashSet<Transaction> Transactions { get; init; }
    required public ImmutableHashSet<ShipyardShip> Ships { get; init; }
    required public int ModificationsFee { get; init; }
}
