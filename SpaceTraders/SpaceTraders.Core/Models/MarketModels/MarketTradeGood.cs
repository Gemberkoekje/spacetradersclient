using SpaceTraders.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Core.Models.MarketModels;

public sealed record MarketTradeGood
{
    required public TradeSymbol Symbol { get; init; }
    required public MarketTradeGoodType Type { get; init; }

    required public int TradeVolume { get; init; }
    required public SupplyLevel Supply { get; init; }
    required public ActivityLevel Activity { get; init; }
    required public int PurchasePrice { get; init; }
    required public int SellPrice { get; init; }
}
