using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Fuel
{
    required public int Current { get; init; }
    required public int Capacity { get; init; }
    required public Consumed Consumed { get; init; }
}
