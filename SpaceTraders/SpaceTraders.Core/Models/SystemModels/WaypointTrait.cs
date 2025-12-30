using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.SystemModels;

/// <summary>
/// Represents a waypoint trait.
/// </summary>
public sealed record WaypointTrait
{
    /// <summary>
    /// Gets the trait symbol.
    /// </summary>
    required public WaypointTraitSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the trait name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the trait description.
    /// </summary>
    required public string Description { get; init; }
}
