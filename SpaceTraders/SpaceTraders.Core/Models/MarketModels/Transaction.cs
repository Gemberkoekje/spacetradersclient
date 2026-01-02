using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using System;

namespace SpaceTraders.Core.Models.MarketModels;

/// <summary>
/// Represents a market transaction.
/// </summary>
public sealed record Transaction
{
    /// <summary>
    /// Gets the waypoint symbol where the transaction occurred.
    /// </summary>
    required public WaypointSymbol WaypointSymbol { get; init; }

    /// <summary>
    /// Gets the ship symbol that made the transaction.
    /// </summary>
    required public ShipSymbol ShipSymbol { get; init; }

    /// <summary>
    /// Gets the trade symbol of the goods.
    /// </summary>
    required public string TradeSymbol { get; init; }

    /// <summary>
    /// Gets the transaction type.
    /// </summary>
    required public MarketTransactionType Type { get; init; }

    /// <summary>
    /// Gets the number of units traded.
    /// </summary>
    required public int Units { get; init; }

    /// <summary>
    /// Gets the price per unit.
    /// </summary>
    required public int PricePerUnit { get; init; }

    /// <summary>
    /// Gets the total price.
    /// </summary>
    required public int TotalPrice { get; init; }

    /// <summary>
    /// Gets the timestamp of the transaction.
    /// </summary>
    required public DateTimeOffset Timestamp { get; init; }
}
