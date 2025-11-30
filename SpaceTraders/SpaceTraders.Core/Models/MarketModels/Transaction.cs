using SpaceTraders.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Core.Models.MarketModels;

public sealed record Transaction
{
    required public string WaypointSymbol { get; init; }
    required public string ShipSymbol { get; init; }
    required public string TradeSymbol { get; init; }
    required public MarketTransactionType Type { get; init; }
    required public int Units { get; init; }
    required public int PricePerUnit { get; init; }
    required public int TotalPrice { get; init; }
    required public DateTimeOffset Timestamp { get; init; }
}
