using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class ShipNavService
{
    private readonly Client.SpaceTradersService _service;
    private readonly ShipService _shipService;

    public ShipNavService(Client.SpaceTradersService service, ShipService shipService)
    {
        _service = service;
        _shipService = shipService;
    }

    public async Task<Result> Dock(string shipSymbol)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));

        if (!ship.CanDock)
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot dock, currently {ship.Navigation.Status}"));

        var result = await _service.EnqueueAsync((client, ct) => client.DockShipAsync(ship.Symbol, ct), true);
        if (!result.IsValid)
            return result;
        return await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
    }

    public async Task<Result> Navigate(string shipSymbol, string waypoint)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));

        if (!ship.CanNavigate)
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot navigate, currently {ship.Navigation.Status}"));

        var result = await _service.EnqueueAsync((client, ct) => client.NavigateShipAsync(new Client.Body6() { WaypointSymbol = waypoint }, ship.Symbol, ct), true);
        if (!result.IsValid)
            return result;
        var result1 = await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
        if (!result1.IsValid)
            return result1;
        return await _shipService.UpdateFuel(result.Value.Data.Fuel, ship.Symbol);
    }

    public async Task<Result> Orbit(string shipSymbol)
    {
        var ship = _shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));

        if (!ship.CanOrbit)
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot orbit, currently {ship.Navigation.Status}"));

        var result = await _service.EnqueueAsync((client, ct) => client.OrbitShipAsync(ship.Symbol, ct), true);
        if (!result.IsValid)
            return result;
        return await _shipService.UpdateNav(result.Value.Data.Nav, ship.Symbol);
    }

}
