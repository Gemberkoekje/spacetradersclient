using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents a ship module.
/// </summary>
public sealed record Module
{
    /// <summary>
    /// Gets the module symbol.
    /// </summary>
    required public ModuleSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the module description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the module capacity.
    /// </summary>
    required public int Capacity { get; init; }

    /// <summary>
    /// Gets the module range.
    /// </summary>
    required public int Range { get; init; }

    /// <summary>
    /// Gets the module requirements.
    /// </summary>
    required public Requirements Requirements { get; init; }
}
