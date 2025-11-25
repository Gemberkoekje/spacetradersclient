using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.FactionModels;

public sealed record Faction
{
    required public FactionSymbol Symbol { get; init; }
    required public string Name { get; init; }

}
