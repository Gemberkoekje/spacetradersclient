using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.Core.Helpers;

public static class Mapper
{
    public static ImmutableList<Mount> MapMounts(ICollection<Client.ShipMount> mounts)
    {
        return mounts.Select(MapMount).ToImmutableList();
    }

    public static Mount MapMount(Client.ShipMount m)
    {
        return new Mount()
        {
            Symbol = m.Symbol.Convert<Client.ShipMountSymbol, MountSymbol>(),
            Name = m.Name,
            Description = m.Description,
            Strength = m.Strength,
            Deposits = m.Deposits != null ? m.Deposits.Select(d => d.Convert<Client.Deposits, Deposits>()).ToImmutableHashSet() : ImmutableHashSet<Deposits>.Empty,
            Requirements = MapRequirements(m.Requirements),
        };
    }

    public static ImmutableList<Module> MapModules(ICollection<Client.ShipModule> modules)
    {
        return modules.Select(MapModule).ToImmutableList();
    }

    public static Module MapModule(Client.ShipModule m)
    {
        return new Module()
        {
            Symbol = m.Symbol.Convert<Client.ShipModuleSymbol, ModuleSymbol>(),
            Name = m.Name,
            Description = m.Description,
            Capacity = m.Capacity,
            Range = m.Range,
            Requirements = MapRequirements(m.Requirements),
        };
    }

    public static Engine MapEngine(Client.ShipEngine engine)
    {
        return new()
        {
            Symbol = engine.Symbol.Convert<Client.ShipEngineSymbol, EngineSymbol>(),
            Name = engine.Name,
            Description = engine.Description,
            Speed = engine.Speed,
            Condition = engine.Condition,
            Integrity = engine.Integrity,
            Requirements = MapRequirements(engine.Requirements),
            Quality = engine.Quality,
        };
    }

    public static Reactor MapReactor(Client.ShipReactor reactor)
    {
        return new()
        {
            Symbol = reactor.Symbol.Convert<Client.ShipReactorSymbol, ReactorSymbol>(),
            Name = reactor.Name,
            Description = reactor.Description,
            PowerOutput = reactor.PowerOutput,
            Condition = reactor.Condition,
            Integrity = reactor.Integrity,
            Requirements = MapRequirements(reactor.Requirements),
            Quality = reactor.Quality,
        };
    }

    public static Frame MapFrame(Client.ShipFrame frame)
    {
        return new()
        {
            Symbol = frame.Symbol.Convert<Client.ShipFrameSymbol, FrameSymbol>(),
            Name = frame.Name,
            Description = frame.Description,
            ModuleSlots = frame.ModuleSlots,
            MountingPoints = frame.MountingPoints,
            FuelCapacity = frame.FuelCapacity,
            Condition = frame.Condition,
            Integrity = frame.Integrity,
            Requirements = MapRequirements(frame.Requirements),
        };
    }

    public static Crew MapCrew(Client.ShipCrew crew)
    {
        return new()
        {
            Current = crew.Current,
            Capacity = crew.Capacity,
            Rotation = crew.Rotation.Convert<Client.ShipCrewRotation, CrewRotation>(),
            Morale = crew.Morale,
            Wages = crew.Wages,
        };
    }

    public static Requirements MapRequirements(Client.ShipRequirements requirements)
    {
        return new()
        {
            Crew = requirements.Crew,
            Power = requirements.Power,
            Slots = requirements.Slots,
        };
    }

}
