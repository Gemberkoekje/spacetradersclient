using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Navigation snapshot describing a ship's current position, planned route, status and flight mode.
/// </summary>
/// <remarks>
/// Values originate from the SpaceTraders API and are mapped into domain enums. The object is immutable and
/// represents a point-in-time view; refreshed by subsequent API calls.
/// </remarks>
public sealed record Navigation
{
    required public string ShipSymbol { get; init; }

    /// <summary>
    /// Symbol of the system the ship is currently in (e.g. system identifier).
    /// </summary>
    required public string SystemSymbol { get; init; }

    /// <summary>
    /// Symbol of the waypoint the ship is currently at or traveling toward.
    /// </summary>
    required public string WaypointSymbol { get; init; }

    /// <summary>
    /// The active navigation route including origin, destination and timing.
    /// </summary>
    required public NavigationRoute Route { get; init; }

    /// <summary>
    /// Current positional status (in transit, in orbit, docked).
    /// </summary>
    required public ShipNavStatus Status { get; init; }

    /// <summary>
    /// Current flight mode affecting speed, fuel use and detectability.
    /// </summary>
    required public ShipNavFlightMode FlightMode { get; init; }
}
