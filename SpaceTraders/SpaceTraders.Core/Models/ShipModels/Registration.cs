using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Registration details of a ship including its name, owning faction and assigned role.
/// </summary>
/// <remarks>
/// Role indicates functional specialization; faction symbol identifies the controlling faction.
/// </remarks>
public sealed record Registration
{
    /// <summary>
    /// The registered name of the ship.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Symbol of the faction the ship belongs to.
    /// </summary>
    required public string FactionSymbol { get; init; }

    /// <summary>
    /// Functional role classification of the ship.
    /// </summary>
    required public ShipRole Role { get; init; }
}
