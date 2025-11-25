using System;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the planned or active navigation route of a ship including origin, destination and timing.
/// </summary>
/// <remarks>
/// Times are expressed as absolute instants in UTC. The route forms part of a navigation snapshot and is immutable.
/// </remarks>
public sealed record NavigationRoute
{
    /// <summary>
    /// The waypoint the ship is traveling toward or has arrived at.
    /// </summary>
    required public NavigationWaypoint Destination { get; init; }

    /// <summary>
    /// The waypoint the ship departed from.
    /// </summary>
    required public NavigationWaypoint Origin { get; init; }

    /// <summary>
    /// The scheduled or actual departure timestamp.
    /// </summary>
    required public DateTimeOffset DepartureTime { get; init; }

    /// <summary>
    /// The estimated or actual arrival timestamp.
    /// </summary>
    required public DateTimeOffset ArrivalTime { get; init; }
}
