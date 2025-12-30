using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents a ship frame.
/// </summary>
public sealed record Frame
{
    /// <summary>
    /// Gets the frame symbol.
    /// </summary>
    required public FrameSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the frame name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the frame condition.
    /// </summary>
    required public double Condition { get; init; }

    /// <summary>
    /// Gets the frame integrity.
    /// </summary>
    required public double Integrity { get; init; }

    /// <summary>
    /// Gets the frame description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the number of module slots.
    /// </summary>
    required public int ModuleSlots { get; init; }

    /// <summary>
    /// Gets the number of mounting points.
    /// </summary>
    required public int MountingPoints { get; init; }

    /// <summary>
    /// Gets the fuel capacity.
    /// </summary>
    required public double FuelCapacity { get; init; }

    /// <summary>
    /// Gets the frame requirements.
    /// </summary>
    required public Requirements Requirements { get; init; }
}
