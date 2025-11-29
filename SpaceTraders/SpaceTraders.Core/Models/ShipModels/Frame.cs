using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Frame
{
    required public FrameSymbol Symbol { get; init; }

    required public string Name { get; init; }

    required public double Condition { get; init; }

    required public double Integrity { get; init; }

    required public string Description { get; init; }

    required public int ModuleSlots { get; init; }

    required public int MountingPoints { get; init; }

    required public double FuelCapacity { get; init; }

    required public Requirements Requirements { get; init; }
}
