using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.SystemModels;

/// <summary>
/// Represents a waypoint modifier.
/// </summary>
public sealed record WaypointModifier
{
    /// <summary>
    /// Gets the modifier symbol.
    /// </summary>
    required public WaypointModifierSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the modifier name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the modifier description.
    /// </summary>
    required public string Description { get; init; }
}
