using Qowaiv;
using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ShipModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Provides higher level ship retrieval operations backed by the generated <c>SpaceTradersClient</c>.
/// </summary>
/// <remarks>
/// Translates the raw client DTO model into the domain <see cref="Ship"/> aggregate with nested value objects
/// and converts generated enum values into internal enums via extension methods.
/// </remarks>
public sealed class ShipService(Client.SpaceTradersService service, ModuleService moduleService)
{
    private ImmutableArray<Ship> Ships { get; set; } = [];

    /// <summary>
    /// Event raised when ships are updated.
    /// </summary>
    public event Func<ImmutableArray<Ship>, Task>? Updated;

    /// <summary>
    /// Event raised when a ship arrival time is noted.
    /// </summary>
    public event Action<DateTimeOffset>? Arrived;

    /// <summary>
    /// Initializes the ship service by fetching ships.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Initialize()
    {
        var ships = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetMyShipsAsync(page, limit, ct),
            page => page.Data);
        var ship = ships.Value.Select(MapShip);
        moduleService.AddEngines(ship.Select(s => s.Engine).ToImmutableArray());
        moduleService.AddReactors(ship.Select(s => s.Reactor).ToImmutableArray());
        moduleService.AddFrames(ship.Select(s => s.Frame).ToImmutableArray());
        moduleService.AddModules(ship.SelectMany(s => s.Modules).ToImmutableArray());
        moduleService.AddMounts(ship.SelectMany(s => s.Mounts).ToImmutableArray());
        Update(ship);
    }

    /// <summary>
    /// Processes ship arrivals.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Arrive()
    {
        if (Ships.IsEmpty)
        {
            return;
        }

        foreach (var ship in Ships.Where(c => c.Navigation.Route.ArrivalTime < Clock.UtcNow().AddSeconds(-1)))
        {
            var update = await service.EnqueueAsync((client, ct) => client.GetMyShipAsync(ship.Symbol, ct));
            var ships = Ships.Remove(ship).Add(MapShip(update.Value.Data));
            Update(ships);
        }
    }

    /// <summary>
    /// Updates a ship's navigation data.
    /// </summary>
    /// <param name="navigation">The navigation data.</param>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators - interface requirement
    public async Task<Result> UpdateNav(Client.ShipNav navigation, string shipSymbol)
#pragma warning restore CS1998
    {
        var ship = Ships.FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with ship symbol {shipSymbol} to update."));
        }

        var updatedShip = ship with { Navigation = MapNavigation(navigation) };
        Update(Ships.Remove(ship).Add(updatedShip));
        return Result.OK;
    }

    /// <summary>
    /// Updates a ship's fuel data.
    /// </summary>
    /// <param name="fuel">The fuel data.</param>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators - interface requirement
    public async Task<Result> UpdateFuel(Client.ShipFuel fuel, string shipSymbol)
#pragma warning restore CS1998
    {
        var ship = Ships.FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with ship symbol {shipSymbol} to update."));
        }

        var updatedShip = ship with { Fuel = MapFuel(fuel) };
        Update(Ships.Remove(ship).Add(updatedShip));
        return Result.OK;
    }

    /// <summary>
    /// Updates a ship's cargo data.
    /// </summary>
    /// <param name="cargo">The cargo data.</param>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators - interface requirement
    public async Task<Result> UpdateCargo(Client.ShipCargo cargo, string shipSymbol)
#pragma warning restore CS1998
    {
        var ship = Ships.FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with ship symbol {shipSymbol} to update."));
        }

        var updatedShip = ship with { Cargo = MapCargo(cargo) };
        Update(Ships.Remove(ship).Add(updatedShip));
        return Result.OK;
    }

    /// <summary>
    /// Gets all ships.
    /// </summary>
    /// <returns>The ships.</returns>
    public ImmutableArray<Ship> GetShips()
    {
        return Ships;
    }

    private void Update(IEnumerable<Ship> ships)
    {
        Ships = [.. ships];
        foreach (var ship in Ships.Where(s => s.Navigation.Route.ArrivalTime > Clock.UtcNow()))
        {
            Arrived?.Invoke(ship.Navigation.Route.ArrivalTime);
        }

        Updated?.Invoke(Ships);
    }

    private static Ship MapShip(Client.Ship ship)
    {
        return new Ship()
        {
            Symbol = ship.Symbol,
            Registration = MapRegistration(ship.Registration),
            Navigation = MapNavigation(ship.Nav),
            Crew = Mapper.MapCrew(ship.Crew),
            Frame = Mapper.MapFrame(ship.Frame),
            Reactor = Mapper.MapReactor(ship.Reactor),
            Engine = Mapper.MapEngine(ship.Engine),
            Modules = Mapper.MapModules(ship.Modules),
            Mounts = Mapper.MapMounts(ship.Mounts),
            Cargo = MapCargo(ship.Cargo),
            Fuel = MapFuel(ship.Fuel),
            Cooldown = MapCooldown(ship.Cooldown),
        };
    }

    private static Cooldown MapCooldown(Client.Cooldown cooldown)
    {
        return new ()
        {
            TotalSeconds = cooldown.TotalSeconds,
            RemainingSeconds = cooldown.RemainingSeconds,
            Expiration = cooldown.Expiration,
        };
    }

    private static Fuel MapFuel(Client.ShipFuel fuel)
    {
        return new ()
        {
            Current = fuel.Current,
            Capacity = fuel.Capacity,
            Consumed = MapConsumed(fuel.Consumed),
        };
    }

    private static Consumed MapConsumed(Client.Consumed consumed)
    {
        return new ()
        {
            Amount = consumed.Amount,
            Timestamp = consumed.Timestamp,
        };
    }

    private static Cargo MapCargo(Client.ShipCargo cargo)
    {
        return new ()
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

    private static Navigation MapNavigation(Client.ShipNav navigation)
    {
        return new ()
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
        return new ()
        {
            Destination = MapWaypoint(route.Destination),
            Origin = MapWaypoint(route.Origin),
            DepartureTime = route.DepartureTime,
            ArrivalTime = route.Arrival,
        };
    }

    private static NavigationWaypoint MapWaypoint(Client.ShipNavRouteWaypoint waypoint)
    {
        return new ()
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
        return new ()
        {
            Name = registration.Name,
            FactionSymbol = registration.FactionSymbol,
            Role = registration.Role.Convert<Client.ShipRole, ShipRole>(),
        };
    }
}
