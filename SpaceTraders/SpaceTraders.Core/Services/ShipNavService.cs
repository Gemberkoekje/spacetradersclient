using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.IDs;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for handling ship navigation operations.
/// </summary>
public sealed class ShipNavService
{
    private readonly Client.SpaceTradersService _service;
    private readonly ShipService _shipService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipNavService"/> class.
    /// </summary>
    /// <param name="service">The SpaceTraders API service.</param>
    /// <param name="shipService">The ship service.</param>
    public ShipNavService(Client.SpaceTradersService service, ShipService shipService)
    {
        _service = service;
        _shipService = shipService;
    }

    /// <summary>
    /// Docks a ship at its current waypoint.
    /// </summary>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> Dock(ShipSymbol shipSymbol)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));
        }

        if (!ship.CanDock)
        {
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot dock, currently {ship.Navigation.Status}"));
        }

        var result = await _service.EnqueueAsync((client, ct) => client.DockShipAsync(ship.Symbol.ToString(), ct), true);
        if (!result.IsValid)
        {
            return result;
        }

        return await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
    }

    /// <summary>
    /// Navigates a ship to a waypoint.
    /// </summary>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <param name="waypoint">The destination waypoint symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> Navigate(ShipSymbol shipSymbol, WaypointSymbol waypoint)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));
        }

        if (!ship.CanNavigate)
        {
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot navigate, currently {ship.Navigation.Status}"));
        }

        var result = await _service.EnqueueAsync((client, ct) => client.NavigateShipAsync(new Client.Body6() { WaypointSymbol = waypoint.ToString() }, ship.Symbol.ToString(), ct), true);
        if (!result.IsValid)
        {
            return result;
        }

        var result1 = await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
        if (!result1.IsValid)
        {
            return result1;
        }

        return await _shipService.UpdateFuel(result.Value.Data.Fuel, ship.Symbol);
    }

    /// <summary>
    /// Puts a ship into orbit.
    /// </summary>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> Orbit(ShipSymbol shipSymbol)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));
        }

        if (!ship.CanOrbit)
        {
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot orbit, currently {ship.Navigation.Status}"));
        }

        var result = await _service.EnqueueAsync((client, ct) => client.OrbitShipAsync(ship.Symbol.ToString(), ct), true);
        if (!result.IsValid)
        {
            return result;
        }

        return await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
    }
}
