using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.Core.Models.ShipyardModels;

public sealed record ShipyardShip
{
    required public ShipType Type { get; init; }
    required public string Name { get; init; }
    required public string Description { get; init; }
    required public ActivityLevel Activity { get; init; }
    required public SupplyLevel Supply { get; init; }
    required public int PurchasePrice { get; init; }
    required public Crew Crew { get; init; }

    required public Frame Frame { get; init; }

    required public Reactor Reactor { get; init; }

    required public Engine Engine { get; init; }

    required public ImmutableList<Module> Modules { get; init; }

    required public ImmutableList<Mount> Mounts { get; init; }

    public int CargoCapacity {
        get => Modules.Where(m =>
        m.Symbol == ModuleSymbol.CargoHoldI ||
        m.Symbol == ModuleSymbol.CargoHoldII ||
        m.Symbol == ModuleSymbol.CargoHoldIII)
            .Sum(m => m.Capacity);
     }
}
