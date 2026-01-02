using SpaceTraders.Core.IDs;

namespace SpaceTraders.Core.Models.ContractModels;

/// <summary>
/// Represents a good to be delivered for a contract.
/// </summary>
public sealed record ContractDeliverGood
{
    /// <summary>
    /// Gets the trade symbol of the good.
    /// </summary>
    required public string TradeSymbol { get; init; }

    /// <summary>
    /// Gets the destination waypoint symbol.
    /// </summary>
    required public WaypointSymbol DestinationSymbol { get; init; }

    /// <summary>
    /// Gets the number of units required.
    /// </summary>
    public int UnitsRequired { get; init; }

    /// <summary>
    /// Gets the number of units fulfilled.
    /// </summary>
    public int UnitsFulfilled { get; init; }
}
