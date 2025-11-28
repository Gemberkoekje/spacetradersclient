using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ShipModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Loaders;

/// <summary>
/// Provides higher level ship retrieval operations backed by the generated <c>SpaceTradersClient</c>.
/// </summary>
/// <remarks>
/// Translates the raw client DTO model into the domain <see cref="Ship"/> aggregate with nested value objects
/// and converts generated enum values into internal enums via extension methods.
/// </remarks>
public sealed class ShipService(Client.SpaceTradersService service)
{

    public async Task<Ship[]> GetMyShips()
    {
        var ships = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetMyShipsAsync(page, limit, ct),
            page => page.Data,
            "GetMyShipsAsync",
            TimeSpan.FromSeconds(60));
        return [.. ships.Value.Select(MapShip)];
    }
    /// <summary>
    /// Retrieves a ship by symbol and maps the client response into a domain <see cref="Ship"/> instance.
    /// </summary>
    /// <param name="symbol">The unique ship symbol to fetch (e.g. agent prefix plus identifier).</param>
    /// <returns>An awaitable task producing the hydrated <see cref="Ship"/>.</returns>
    /// <exception cref="Client.ApiException">Thrown when the API request fails.</exception>
    public async Task<Ship> GetShip(string symbol)
    {
        var ship = await service.EnqueueCachedAsync((client, ct) => client.GetMyShipAsync(symbol, ct), $"GetMyShipAsync_{symbol}", TimeSpan.FromSeconds(60));
        return MapShip(ship.Value.Data);
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
