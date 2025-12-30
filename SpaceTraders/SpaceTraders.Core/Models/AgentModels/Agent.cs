namespace SpaceTraders.Core.Models.AgentModels;

/// <summary>
/// Represents an agent in the game.
/// </summary>
public sealed record Agent
{
    /// <summary>
    /// Gets the agent symbol.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Gets the agent's headquarters location.
    /// </summary>
    required public string Headquarters { get; init; }

    /// <summary>
    /// Gets the agent's credit balance.
    /// </summary>
    required public long Credits { get; init; }

    /// <summary>
    /// Gets the agent's starting faction.
    /// </summary>
    required public string StartingFaction { get; init; }

    /// <summary>
    /// Gets the number of ships the agent owns.
    /// </summary>
    required public int ShipCount { get; init; }
}
