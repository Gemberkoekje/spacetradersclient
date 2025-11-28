using Qowaiv;
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
            Registration = new()
            {
                Name = ship.Registration.Name,
                FactionSymbol = ship.Registration.FactionSymbol,
                Role = ship.Registration.Role.Convert(),
            },
            Navigation = new()
            {
                ShipSymbol = ship.Symbol,
                SystemSymbol = ship.Nav.SystemSymbol,
                WaypointSymbol = ship.Nav.WaypointSymbol,
                Route = new()
                {
                    Destination = new()
                    {
                        Symbol = ship.Nav.Route.Destination.Symbol,
                        Type = ship.Nav.Route.Destination.Type.Convert(),
                        SystemSymbol = ship.Nav.Route.Destination.SystemSymbol,
                        X = ship.Nav.Route.Destination.X,
                        Y = ship.Nav.Route.Destination.Y,
                    },
                    Origin = new()
                    {
                        Symbol = ship.Nav.Route.Origin.Symbol,
                        Type = ship.Nav.Route.Origin.Type.Convert(),
                        SystemSymbol = ship.Nav.Route.Origin.SystemSymbol,
                        X = ship.Nav.Route.Origin.X,
                        Y = ship.Nav.Route.Origin.Y,
                    },
                    DepartureTime = ship.Nav.Route.DepartureTime,
                    ArrivalTime = ship.Nav.Route.Arrival,
                },
                Status = ship.Nav.Status.Convert(),
                FlightMode = ship.Nav.FlightMode.Convert(),
            },
        };
    }
}
