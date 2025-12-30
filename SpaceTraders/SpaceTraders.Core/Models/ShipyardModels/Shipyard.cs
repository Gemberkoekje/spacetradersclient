using SpaceTraders.Core.Enums;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipyardModels;

/// <summary>
/// Represents a shipyard at a waypoint.
/// </summary>
public sealed record Shipyard
{
    /// <summary>
    /// Gets the shipyard symbol.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Gets the ship types available at this shipyard.
    /// </summary>
    required public ImmutableHashSet<ShipType> ShipTypes { get; init; }

    /// <summary>
    /// Gets the transactions at this shipyard.
    /// </summary>
    required public ImmutableHashSet<Transaction> Transactions { get; init; }

    /// <summary>
    /// Gets the ships available for purchase at this shipyard.
    /// </summary>
    required public ImmutableHashSet<ShipyardShip> Ships { get; init; }

    /// <summary>
    /// Gets the modifications fee.
    /// </summary>
    required public int ModificationsFee { get; init; }
}
