using System;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents fuel consumption data.
/// </summary>
public sealed record Consumed
{
    /// <summary>
    /// Gets the amount of fuel consumed.
    /// </summary>
    required public int Amount { get; init; }

    /// <summary>
    /// Gets the timestamp of the consumption.
    /// </summary>
    required public DateTimeOffset Timestamp { get; init; }
}
