using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.SystemModels;

public sealed record WaypointModifier
{
    required public WaypointModifierSymbol Symbol { get; init; }
    required public string Name { get; init; }
    required public string Description { get; init; }
}
