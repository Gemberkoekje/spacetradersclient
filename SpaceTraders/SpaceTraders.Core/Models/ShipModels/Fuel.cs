namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the fuel state of a ship.
/// </summary>
public sealed record Fuel
{
    /// <summary>
    /// Gets the current fuel amount.
    /// </summary>
    required public int Current { get; init; }

    /// <summary>
    /// Gets the fuel capacity.
    /// </summary>
    required public int Capacity { get; init; }

    /// <summary>
    /// Gets the fuel consumption data.
    /// </summary>
    required public Consumed Consumed { get; init; }
}
