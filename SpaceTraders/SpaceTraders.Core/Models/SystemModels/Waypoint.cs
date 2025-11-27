using SpaceTraders.Core.Enums;
using System.Collections.Generic;

namespace SpaceTraders.Core.Models.SystemModels;

public sealed record Waypoint
{
    required public string SystemSymbol { get; init; } 
    required public string Symbol { get; init; }
    required public WaypointType Type { get; init; }
    required public int X { get; init; }
    required public int Y { get; init; }
    required public IList<string> Orbitals { get; init; }
    required public string Orbits { get; init; }
    required public IList<WaypointTrait> Traits { get; init; }
    required public IList<WaypointModifier> Modifiers { get; init; }
    required public Chart? Chart { get; init; }
    required public bool IsUnderConstruction { get; init; }

}
