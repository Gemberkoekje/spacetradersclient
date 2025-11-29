using SpaceTraders.Core.Enums;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Mount
{
    required public MountSymbol Symbol { get; init; }
    required public string Name { get; init; }
    required public string Description { get; init; }
    required public int Strength { get; init; }
    required public ImmutableHashSet<Deposits> Deposits { get; init; }
    required public Requirements Requirements { get; init; }
}
