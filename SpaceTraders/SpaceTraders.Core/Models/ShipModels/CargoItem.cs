using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record CargoItem
{
    required public TradeSymbol Symbol { get; init; }

    required public string Name { get; init; }

    required public string Description { get; init; }

    required public int Units { get; init; }
}
