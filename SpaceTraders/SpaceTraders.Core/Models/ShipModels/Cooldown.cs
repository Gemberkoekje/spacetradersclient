using System;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the cooldown state of a ship.
/// </summary>
public sealed record Cooldown
{
    /// <summary>
    /// Gets the total cooldown duration in seconds.
    /// </summary>
    required public int TotalSeconds { get; init; }

    /// <summary>
    /// Gets the remaining cooldown time in seconds.
    /// </summary>
    required public int RemainingSeconds { get; init; }

    /// <summary>
    /// Gets the expiration time of the cooldown.
    /// </summary>
    required public DateTimeOffset Expiration { get; init; }
}
