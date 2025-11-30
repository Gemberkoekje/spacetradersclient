using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.MarketModels;

public sealed record TradeGood
{
    required public TradeSymbol Symbol { get; init; }
    required public string Name { get; init; }
    required public string Description { get; init; }
}
