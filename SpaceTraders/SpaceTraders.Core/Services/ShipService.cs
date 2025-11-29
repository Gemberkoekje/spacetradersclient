using Qowaiv;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ShipModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Provides higher level ship retrieval operations backed by the generated <c>SpaceTradersClient</c>.
/// </summary>
/// <remarks>
/// Translates the raw client DTO model into the domain <see cref="Ship"/> aggregate with nested value objects
/// and converts generated enum values into internal enums via extension methods.
/// </remarks>
public sealed class ShipService(Client.SpaceTradersService service)
{
    private ImmutableList<Ship> Ships { get; set; } = [];

    public event Func<Ship[], Task>? Updated;

    public event Action<DateTimeOffset>? Arrived;

    public async Task Initialize()
    {
        var ships = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetMyShipsAsync(page, limit, ct),
            page => page.Data);
        Update(ships.Value.Select(MapShip));
    }

    private void Update(IEnumerable<Ship> ships)
    {
        Ships = ships.ToImmutableList();
        foreach (var ship in Ships.Where(s => s.Navigation.Route.ArrivalTime > Clock.UtcNow()))
        {
            Arrived?.Invoke(ship.Navigation.Route.ArrivalTime);
        }
        Updated?.Invoke(ships.ToArray());
    }

    public async Task Arrive()
    {
        if (Ships.IsEmpty)
            return;
        foreach (var ship in Ships.Where(c => c.Navigation.Route.ArrivalTime < Clock.UtcNow().AddSeconds(-1)))
        {
            var update = await service.EnqueueAsync((client, ct) => client.GetMyShipAsync(ship.Symbol, ct));
            var ships = Ships.Remove(ship).Add(MapShip(update.Value.Data));
            Update(ships);
        }
    }

    public ImmutableList<Ship> GetShips()
    {
        return Ships;
    }

    private static Ship MapShip(Client.Ship ship)
    {
        return new Ship()
        {
            Symbol = ship.Symbol,
            Registration = MapRegistration(ship.Registration),
            Navigation = MapNavigation(ship.Nav),
            Crew = MapCrew(ship.Crew),
            Frame = MapFrame(ship.Frame),
            Reactor = MapReactor(ship.Reactor),
            Engine = MapEngine(ship.Engine),
            Modules = MapModules(ship.Modules),
            Mounts = MapMounts(ship.Mounts),
            Cargo = MapCargo(ship.Cargo),
            Fuel = MapFuel(ship.Fuel),
            Cooldown = MapCooldown(ship.Cooldown),
        };
    }

    private static Cooldown MapCooldown(Client.Cooldown cooldown)
    {
        return new()
        {
            TotalSeconds = cooldown.TotalSeconds,
            RemainingSeconds = cooldown.RemainingSeconds,
            Expiration = cooldown.Expiration,
        };
    }

    private static Fuel MapFuel(Client.ShipFuel fuel)
    {
        return new()
        {
            Current = fuel.Current,
            Capacity = fuel.Capacity,
            Consumed = MapConsumed(fuel.Consumed),
        };
    }

    private static Consumed MapConsumed(Client.Consumed consumed)
    {
        return new()
        {
            Amount = consumed.Amount,
            Timestamp = consumed.Timestamp,
        };
    }

    private static Cargo MapCargo(Client.ShipCargo cargo)
    {
        return new()
        {
            Capacity = cargo.Capacity,
            Units = cargo.Units,
            Inventory = MapInventory(cargo.Inventory),
        };
    }

    private static ImmutableHashSet<CargoItem> MapInventory(ICollection<Client.ShipCargoItem> inventory)
    {
        return inventory.Select(MapCargoItem).ToImmutableHashSet();
    }

    private static CargoItem MapCargoItem(Client.ShipCargoItem i)
    {
        return new CargoItem()
        {
            Symbol = i.Symbol.Convert<Client.TradeSymbol, TradeSymbol>(),
            Name = i.Name,
            Description = i.Description,
            Units = i.Units,
        };
    }

    private static ImmutableList<Mount> MapMounts(ICollection<Client.ShipMount> mounts)
    {
        return mounts.Select(MapMount).ToImmutableList();
    }

    private static Mount MapMount(Client.ShipMount m)
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

    private static ImmutableList<Module> MapModules(ICollection<Client.ShipModule> modules)
    {
        return modules.Select(MapModule).ToImmutableList();
    }

    private static Module MapModule(Client.ShipModule m)
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

    private static Engine MapEngine(Client.ShipEngine engine)
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

    private static Reactor MapReactor(Client.ShipReactor reactor)
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

    private static Frame MapFrame(Client.ShipFrame frame)
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

    private static Crew MapCrew(Client.ShipCrew crew)
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

    private static Navigation MapNavigation(Client.ShipNav navigation)
    {
        return new()
        {
            SystemSymbol = navigation.SystemSymbol,
            WaypointSymbol = navigation.WaypointSymbol,
            Route = MapRoute(navigation.Route),
            Status = navigation.Status.Convert<Client.ShipNavStatus, ShipNavStatus>(),
            FlightMode = navigation.FlightMode.Convert<Client.ShipNavFlightMode, ShipNavFlightMode>(),
        };
    }

    private static NavigationRoute MapRoute(Client.ShipNavRoute route)
    {
        return new()
        {
            Destination = MapWaypoint(route.Destination),
            Origin = MapWaypoint(route.Origin),
            DepartureTime = route.DepartureTime,
            ArrivalTime = route.Arrival,
        };
    }

    private static NavigationWaypoint MapWaypoint(Client.ShipNavRouteWaypoint waypoint)
    {
        return new()
        {
            Symbol = waypoint.Symbol,
            Type = waypoint.Type.Convert<Client.WaypointType, WaypointType>(),
            SystemSymbol = waypoint.SystemSymbol,
            X = waypoint.X,
            Y = waypoint.Y,
        };
    }

    private static Registration MapRegistration(Client.ShipRegistration registration)
    {
        return new()
        {
            Name = registration.Name,
            FactionSymbol = registration.FactionSymbol,
            Role = registration.Role.Convert<Client.ShipRole, ShipRole>(),
        };
    }

    private static Requirements MapRequirements(Client.ShipRequirements requirements)
    {
        return new()
        {
            Crew = requirements.Crew,
            Power = requirements.Power,
            Slots = requirements.Slots,
        };
    }
}
