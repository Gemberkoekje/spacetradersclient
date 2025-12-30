using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the crew of a ship.
/// </summary>
public sealed record Crew
{
    /// <summary>
    /// Gets the current crew count.
    /// </summary>
    required public int Current { get; init; }

    /// <summary>
    /// Gets the crew capacity.
    /// </summary>
    required public int Capacity { get; init; }

    /// <summary>
    /// Gets the crew rotation schedule.
    /// </summary>
    required public CrewRotation Rotation { get; init; }

    /// <summary>
    /// Gets the crew morale level.
    /// </summary>
    required public int Morale { get; init; }

    /// <summary>
    /// Gets the crew wages.
    /// </summary>
    required public int Wages { get; init; }
}
