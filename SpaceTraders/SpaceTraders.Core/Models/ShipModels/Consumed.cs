using System;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Consumed
{
    required public int Amount { get; init; }
    required public DateTimeOffset Timestamp { get; init; }
}
