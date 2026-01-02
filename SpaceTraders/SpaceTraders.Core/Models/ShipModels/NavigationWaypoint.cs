using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Describes a waypoint involved in a ship's navigation route including spatial coordinates and classification.
/// </summary>
/// <remarks>
/// Waypoints are referenced by a unique symbol scoped to a system. The <see cref="Type"/> determines available
/// interactions and resource characteristics.
/// </remarks>
public sealed record NavigationWaypoint
{
    /// <summary>
    /// Unique waypoint symbol identifier.
    /// </summary>
    required public WaypointSymbol Symbol { get; init; }

    /// <summary>
    /// The category of waypoint (planet, station, asteroid, etc.).
    /// </summary>
    required public WaypointType Type { get; init; }

    /// <summary>
    /// Symbol of the system this waypoint belongs to.
    /// </summary>
    required public SystemSymbol SystemSymbol { get; init; }

    /// <summary>
    /// X coordinate within the system grid.
    /// </summary>
    required public int X { get; init; }

    /// <summary>
    /// Y coordinate within the system grid.
    /// </summary>
    required public int Y { get; init; }
}
