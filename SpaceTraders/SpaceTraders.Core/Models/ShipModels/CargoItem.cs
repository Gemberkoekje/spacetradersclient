using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents an item in the ship's cargo.
/// </summary>
public sealed record CargoItem
{
    /// <summary>
    /// Gets the trade symbol of the cargo item.
    /// </summary>
    required public TradeSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the name of the cargo item.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the description of the cargo item.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the number of units of the cargo item.
    /// </summary>
    required public int Units { get; init; }
}
