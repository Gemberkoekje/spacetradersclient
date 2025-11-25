
using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.SystemModels;

public sealed record WaypointTrait
{
    required public WaypointTraitSymbol Symbol { get; init; }
    required public string Name { get; init; }
    required public string Description { get; init; }
}
