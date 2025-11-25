namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Aggregate root representing a SpaceTraders ship with identity, registration and navigation state.
/// </summary>
/// <remarks>
/// Composed of registration metadata and a navigation snapshot. Additional facets (cargo, modules, crew)
/// may be added as the domain model expands.
/// </remarks>
public sealed record Ship
{
    /// <summary>
    /// Unique ship symbol identifier.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Registration details (name, faction and role).
    /// </summary>
    required public Registration Registration { get; init; }

    /// <summary>
    /// Current navigation snapshot including route, positional status and flight mode.
    /// </summary>
    required public Navigation Navigation { get; init; }

}
