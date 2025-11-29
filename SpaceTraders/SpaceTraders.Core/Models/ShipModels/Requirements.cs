namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Requirements
{
    required public int Power { get; init; }
    required public int Crew { get; init; }
    required public int Slots { get; init; }
}
