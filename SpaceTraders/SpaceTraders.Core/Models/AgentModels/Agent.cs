namespace SpaceTraders.Core.Models.AgentModels;

public sealed record Agent
{
    required public string Symbol { get; init; }

    required public string Headquarters { get; init; }

    required public long Credits { get; init; }

    required public string StartingFaction { get; init; }

    required public int ShipCount { get; init; }
}
