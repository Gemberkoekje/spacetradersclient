using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents a ship reactor.
/// </summary>
public sealed record Reactor
{
    /// <summary>
    /// Gets the reactor symbol.
    /// </summary>
    required public ReactorSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the reactor name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the reactor condition.
    /// </summary>
    required public double Condition { get; init; }

    /// <summary>
    /// Gets the reactor integrity.
    /// </summary>
    required public double Integrity { get; init; }

    /// <summary>
    /// Gets the reactor description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the reactor power output.
    /// </summary>
    required public int PowerOutput { get; init; }

    /// <summary>
    /// Gets the reactor requirements.
    /// </summary>
    required public Requirements Requirements { get; init; }

    /// <summary>
    /// Gets the reactor quality.
    /// </summary>
    required public double Quality { get; init; }
}
