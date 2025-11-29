using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Module
{
    required public ModuleSymbol Symbol { get; init; }

    required public string Name { get; init; }

    required public string Description { get; init; }

    required public int Capacity { get; init; }

    required public int Range { get; init; }

    required public Requirements Requirements { get; init; }
}
