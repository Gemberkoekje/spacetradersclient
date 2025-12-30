using SpaceTraders.Core.Enums;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.SystemModels;

/// <summary>
/// Represents a waypoint in a system.
/// </summary>
public sealed record Waypoint
{
    /// <summary>
    /// Gets the system symbol.
    /// </summary>
    required public string SystemSymbol { get; init; }

    /// <summary>
    /// Gets the waypoint symbol.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Gets the waypoint type.
    /// </summary>
    required public WaypointType Type { get; init; }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    required public int X { get; init; }

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    required public int Y { get; init; }

    /// <summary>
    /// Gets the orbital waypoint symbols.
    /// </summary>
    required public ImmutableArray<string> Orbitals { get; init; }

    /// <summary>
    /// Gets the symbol of the waypoint this orbits.
    /// </summary>
    required public string Orbits { get; init; }

    /// <summary>
    /// Gets the waypoint traits.
    /// </summary>
    required public ImmutableArray<WaypointTrait> Traits { get; init; }

    /// <summary>
    /// Gets the waypoint modifiers.
    /// </summary>
    required public ImmutableArray<WaypointModifier> Modifiers { get; init; }

    /// <summary>
    /// Gets the waypoint chart.
    /// </summary>
    required public Chart? Chart { get; init; }

    /// <summary>
    /// Gets a value indicating whether the waypoint is under construction.
    /// </summary>
    required public bool IsUnderConstruction { get; init; }
}
