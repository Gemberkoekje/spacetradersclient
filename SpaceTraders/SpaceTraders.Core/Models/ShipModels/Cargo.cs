using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the cargo hold of a ship.
/// </summary>
public sealed record Cargo
{
    /// <summary>
    /// Gets the cargo capacity.
    /// </summary>
    required public int Capacity { get; init; }

    /// <summary>
    /// Gets the number of units currently in cargo.
    /// </summary>
    required public int Units { get; init; }

    /// <summary>
    /// Gets the inventory of cargo items.
    /// </summary>
    required public ImmutableHashSet<CargoItem> Inventory { get; init; }
}
