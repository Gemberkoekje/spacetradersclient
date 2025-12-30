namespace SpaceTraders.Core.Models.ShipyardModels;

/// <summary>
/// Represents crew requirements for a shipyard ship.
/// </summary>
public sealed record Crew
{
    /// <summary>
    /// Gets the required crew count.
    /// </summary>
    required public int Required { get; init; }

    /// <summary>
    /// Gets the crew capacity.
    /// </summary>
    required public int Capacity { get; init; }
}
