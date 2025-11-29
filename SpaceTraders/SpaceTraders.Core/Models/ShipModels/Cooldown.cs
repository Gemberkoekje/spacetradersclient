using System;

namespace SpaceTraders.Core.Models.ShipModels;

public sealed record Cooldown
{
    required public int TotalSeconds { get; init; }

    required public int RemainingSeconds { get; init; }

    required public DateTimeOffset Expiration { get; init; }
}
