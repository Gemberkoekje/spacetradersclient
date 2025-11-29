using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Crew
{
    required public int Current { get; init; }

    required public int Capacity { get; init; }

    required public CrewRotation Rotation { get; init; }

    required public int Morale { get; init; }

    required public int Wages { get; init; }
}
