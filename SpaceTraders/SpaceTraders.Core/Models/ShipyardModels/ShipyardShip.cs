using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.Core.Models.ShipyardModels;

/// <summary>
/// Represents a ship available at a shipyard.
/// </summary>
public sealed record ShipyardShip
{
    /// <summary>
    /// Gets the ship type.
    /// </summary>
    required public ShipType Type { get; init; }

    /// <summary>
    /// Gets the ship name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the ship description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets the activity level.
    /// </summary>
    required public ActivityLevel Activity { get; init; }

    /// <summary>
    /// Gets the supply level.
    /// </summary>
    required public SupplyLevel Supply { get; init; }

    /// <summary>
    /// Gets the purchase price.
    /// </summary>
    required public int PurchasePrice { get; init; }

    /// <summary>
    /// Gets the crew information.
    /// </summary>
    required public Crew Crew { get; init; }

    /// <summary>
    /// Gets the ship frame.
    /// </summary>
    required public Frame Frame { get; init; }

    /// <summary>
    /// Gets the ship reactor.
    /// </summary>
    required public Reactor Reactor { get; init; }

    /// <summary>
    /// Gets the ship engine.
    /// </summary>
    required public Engine Engine { get; init; }

    /// <summary>
    /// Gets the ship modules.
    /// </summary>
    required public ImmutableArray<Module> Modules { get; init; }

    /// <summary>
    /// Gets the ship mounts.
    /// </summary>
    required public ImmutableArray<Mount> Mounts { get; init; }

    /// <summary>
    /// Gets the calculated cargo capacity.
    /// </summary>
    public int CargoCapacity
    {
        get => Modules.Where(m =>
            m.Symbol == ModuleSymbol.CargoHoldI ||
            m.Symbol == ModuleSymbol.CargoHoldII ||
            m.Symbol == ModuleSymbol.CargoHoldIII)
            .Sum(m => m.Capacity);
    }
}
