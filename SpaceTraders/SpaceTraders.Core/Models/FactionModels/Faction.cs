using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.FactionModels;

/// <summary>
/// Represents a faction in the game.
/// </summary>
public sealed record Faction
{
    /// <summary>
    /// Gets the faction symbol.
    /// </summary>
    required public FactionSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the faction name.
    /// </summary>
    required public string Name { get; init; }
}
