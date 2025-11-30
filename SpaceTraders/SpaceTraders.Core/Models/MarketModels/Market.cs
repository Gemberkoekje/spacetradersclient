using SpaceTraders.Core.Models.ShipyardModels;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.MarketModels;

public sealed record Market
{
    required public string Symbol { get; init; }
    required public ImmutableHashSet<TradeGood> Exports { get; init; }
    required public ImmutableHashSet<TradeGood> Imports { get; init; }
    required public ImmutableHashSet<TradeGood> Exchange { get; init; }
    required public ImmutableHashSet<Transaction> Transactions { get; init; }
    required public ImmutableHashSet<MarketTradeGood> TradeGoods { get; init; }
}
