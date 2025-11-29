using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Core.Models.ShipyardModels;

public sealed record Transaction
{
    required public string WaypointSymbol { get; init; }
    required public string ShipType { get; init; }
    required public int Price { get; init; }
    required public string AgentSymbol { get; init; }
    required public DateTimeOffset Timestamp { get; init; }
}
