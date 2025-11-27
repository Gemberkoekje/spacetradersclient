using SpaceTraders.Core.Enums;
using System.Collections.Generic;

namespace SpaceTraders.Core.Models.SystemModels;

public sealed record SystemWaypoint
{
    required public string Constellation { get; init; }
    required public string Symbol { get; init; }
    required public string SectorSymbol { get; init; }
    required public SystemType SystemType { get; init; }
    required public int X { get; init; }
    required public int Y { get; init; }
    required public IEnumerable<Waypoint> Waypoints { get; init; }
    required public IEnumerable<FactionSymbol> Factions { get; init; }
    required public string Name { get; init; }

}
