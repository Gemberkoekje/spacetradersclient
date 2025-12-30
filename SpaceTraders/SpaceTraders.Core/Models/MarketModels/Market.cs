using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.MarketModels;

/// <summary>
/// Represents a market at a waypoint.
/// </summary>
public sealed record Market
{
    /// <summary>
    /// Gets the market symbol.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Gets the goods exported by this market.
    /// </summary>
    required public ImmutableHashSet<TradeGood> Exports { get; init; }

    /// <summary>
    /// Gets the goods imported by this market.
    /// </summary>
    required public ImmutableHashSet<TradeGood> Imports { get; init; }

    /// <summary>
    /// Gets the goods exchanged at this market.
    /// </summary>
    required public ImmutableHashSet<TradeGood> Exchange { get; init; }

    /// <summary>
    /// Gets the transactions at this market.
    /// </summary>
    required public ImmutableHashSet<Transaction> Transactions { get; init; }

    /// <summary>
    /// Gets the trade goods available at this market.
    /// </summary>
    required public ImmutableHashSet<MarketTradeGood> TradeGoods { get; init; }
}
