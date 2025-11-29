using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Engine
{
    required public EngineSymbol Symbol { get; init; }
    required public string Name { get; init; }
    required public double Condition { get; init; }
    required public double Integrity { get; init; }
    required public string Description { get; init; }
    required public int Speed { get; init; }
    required public Requirements Requirements { get; init; }
    required public double Quality { get; init; }
}
