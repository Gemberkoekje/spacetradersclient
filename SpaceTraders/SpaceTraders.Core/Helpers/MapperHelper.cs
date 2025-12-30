using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.Core.Helpers;

/// <summary>
/// Provides mapping methods to convert client models to domain models.
/// </summary>
public static class Mapper
{
    /// <summary>
    /// Maps a collection of ship mounts from client to domain model.
    /// </summary>
    /// <param name="mounts">The client ship mounts.</param>
    /// <returns>An immutable array of domain mounts.</returns>
    public static ImmutableArray<Mount> MapMounts(ICollection<Client.ShipMount> mounts)
    {
        return [.. mounts.Select(MapMount)];
    }

    /// <summary>
    /// Maps a ship mount from client to domain model.
    /// </summary>
    /// <param name="m">The client ship mount.</param>
    /// <returns>The domain mount.</returns>
    public static Mount MapMount(Client.ShipMount m)
    {
        return new Mount()
        {
            Symbol = m.Symbol.Convert<Client.ShipMountSymbol, MountSymbol>(),
            Name = m.Name,
            Description = m.Description,
            Strength = m.Strength,
            Deposits = m.Deposits != null ? [.. m.Deposits.Select(d => d.Convert<Client.Deposits, Deposits>())] : [],
            Requirements = MapRequirements(m.Requirements),
        };
    }

    /// <summary>
    /// Maps a collection of ship modules from client to domain model.
    /// </summary>
    /// <param name="modules">The client ship modules.</param>
    /// <returns>An immutable array of domain modules.</returns>
    public static ImmutableArray<Module> MapModules(ICollection<Client.ShipModule> modules)
    {
        return [.. modules.Select(MapModule)];
    }

    /// <summary>
    /// Maps a ship module from client to domain model.
    /// </summary>
    /// <param name="m">The client ship module.</param>
    /// <returns>The domain module.</returns>
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

    /// <summary>
    /// Maps a ship engine from client to domain model.
    /// </summary>
    /// <param name="engine">The client ship engine.</param>
    /// <returns>The domain engine.</returns>
    public static Engine MapEngine(Client.ShipEngine engine)
    {
        return new ()
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

    /// <summary>
    /// Maps a ship reactor from client to domain model.
    /// </summary>
    /// <param name="reactor">The client ship reactor.</param>
    /// <returns>The domain reactor.</returns>
    public static Reactor MapReactor(Client.ShipReactor reactor)
    {
        return new ()
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

    /// <summary>
    /// Maps a ship frame from client to domain model.
    /// </summary>
    /// <param name="frame">The client ship frame.</param>
    /// <returns>The domain frame.</returns>
    public static Frame MapFrame(Client.ShipFrame frame)
    {
        return new ()
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

    /// <summary>
    /// Maps ship crew from client to domain model.
    /// </summary>
    /// <param name="crew">The client ship crew.</param>
    /// <returns>The domain crew.</returns>
    public static Crew MapCrew(Client.ShipCrew crew)
    {
        return new ()
        {
            Current = crew.Current,
            Capacity = crew.Capacity,
            Rotation = crew.Rotation.Convert<Client.ShipCrewRotation, CrewRotation>(),
            Morale = crew.Morale,
            Wages = crew.Wages,
        };
    }

    /// <summary>
    /// Maps ship requirements from client to domain model.
    /// </summary>
    /// <param name="requirements">The client ship requirements.</param>
    /// <returns>The domain requirements.</returns>
    public static Requirements MapRequirements(Client.ShipRequirements requirements)
    {
        return new ()
        {
            Crew = requirements.Crew,
            Power = requirements.Power,
            Slots = requirements.Slots,
        };
    }
}
