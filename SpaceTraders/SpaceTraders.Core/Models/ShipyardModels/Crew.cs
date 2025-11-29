using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Core.Models.ShipyardModels;

public sealed record Crew
{
    required public int Required { get; init; }
    required public int Capacity { get; init; }
}
