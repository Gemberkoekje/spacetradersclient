using SpaceTraders.Core.Enums;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents a ship mount.
/// </summary>
public sealed record Mount
{
    /// <summary>
    /// Gets the mount symbol.
    /// </summary>
    required public MountSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the mount name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the mount description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the mount strength.
    /// </summary>
    required public int Strength { get; init; }

    /// <summary>
    /// Gets the deposits the mount can extract.
    /// </summary>
    required public ImmutableHashSet<Deposits> Deposits { get; init; }

    /// <summary>
    /// Gets the mount requirements.
    /// </summary>
    required public Requirements Requirements { get; init; }
}
