using SpaceTraders.Core.IDs;
using System;

namespace SpaceTraders.Core.Models.ShipyardModels;

/// <summary>
/// Represents a shipyard transaction.
/// </summary>
public sealed record Transaction
{
    /// <summary>
    /// Gets the waypoint symbol.
    /// </summary>
    required public WaypointSymbol WaypointSymbol { get; init; }

    /// <summary>
    /// Gets the ship type.
    /// </summary>
    required public string ShipType { get; init; }

    /// <summary>
    /// Gets the transaction price.
    /// </summary>
    required public int Price { get; init; }

    /// <summary>
    /// Gets the agent symbol.
    /// </summary>
    required public AgentSymbol AgentSymbol { get; init; }

    /// <summary>
    /// Gets the transaction timestamp.
    /// </summary>
    required public DateTimeOffset Timestamp { get; init; }
}
