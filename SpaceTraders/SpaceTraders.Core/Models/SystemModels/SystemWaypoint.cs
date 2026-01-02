using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.SystemModels;

/// <summary>
/// Represents a system in the galaxy.
/// </summary>
public sealed record SystemWaypoint
{
    /// <summary>
    /// Gets the constellation.
    /// </summary>
    required public string Constellation { get; init; }

    /// <summary>
    /// Gets the system symbol.
    /// </summary>
    required public SystemSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the sector symbol.
    /// </summary>
    required public SectorSymbol SectorSymbol { get; init; }

    /// <summary>
    /// Gets the system type.
    /// </summary>
    required public SystemType SystemType { get; init; }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    required public int X { get; init; }

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    required public int Y { get; init; }

    /// <summary>
    /// Gets the factions present in the system.
    /// </summary>
    required public ImmutableArray<FactionSymbol> Factions { get; init; }

    /// <summary>
    /// Gets the system name.
    /// </summary>
    required public string Name { get; init; }
}
