using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents a ship engine.
/// </summary>
public sealed record Engine
{
    /// <summary>
    /// Gets the engine symbol.
    /// </summary>
    required public EngineSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the engine name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the engine condition.
    /// </summary>
    required public double Condition { get; init; }

    /// <summary>
    /// Gets the engine integrity.
    /// </summary>
    required public double Integrity { get; init; }

    /// <summary>
    /// Gets the engine description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the engine speed.
    /// </summary>
    required public int Speed { get; init; }

    /// <summary>
    /// Gets the engine requirements.
    /// </summary>
    required public Requirements Requirements { get; init; }

    /// <summary>
    /// Gets the engine quality.
    /// </summary>
    required public double Quality { get; init; }
}
