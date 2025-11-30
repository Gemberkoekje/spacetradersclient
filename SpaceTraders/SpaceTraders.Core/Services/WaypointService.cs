using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class WaypointService(Client.SpaceTradersService service)
{
    private ImmutableDictionary<string, ImmutableList<Waypoint>> Waypoints { get; set; } = [];

    public event Func<ImmutableDictionary<string, ImmutableList<Waypoint>>, Task>? Updated;
    public async Task Initialize()
    {
    }

    private void Update(ImmutableDictionary<string, ImmutableList<Waypoint>> waypoints)
    {
        Waypoints = waypoints;
        Updated?.Invoke(waypoints);
    }

    public async Task AddSystem(string symbol)
    {
        var systemWaypoints = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetSystemWaypointsAsync(page, limit, null, null, symbol, ct),
            page => page.Data);
        var waypoints = systemWaypoints.Value.Select(w => MapWaypoint(w));
        var newDict = Waypoints.SetItem(symbol, waypoints.ToImmutableList());
        Update(newDict);
    }

    public ImmutableDictionary<string, ImmutableList<Waypoint>> GetWaypoints()
    {
        return Waypoints;
    }

    private static Waypoint MapWaypoint(Client.Waypoint w)
    {
        return new Waypoint()
        {
            SystemSymbol = w.SystemSymbol,
            Symbol = w.Symbol,
            Type = w.Type.Convert<Client.WaypointType, WaypointType>(),
            X = w.X,
            Y = w.Y,
            Orbitals = w.Orbitals.Select(o => o.Symbol).ToList(),
            Orbits = w.Orbits,
            Traits = [.. w.Traits.Select(t => new WaypointTrait() { Name = t.Name, Description = t.Description, Symbol = t.Symbol.Convert<Client.WaypointTraitSymbol, Enums.WaypointTraitSymbol>() })],
            Modifiers = [.. w.Modifiers.Select(m => new WaypointModifier() { Name = m.Name, Description = m.Description, Symbol = m.Symbol.Convert<Client.WaypointModifierSymbol, WaypointModifierSymbol>() })],
            Chart = new Chart() { WaypointSymbol = w.Chart.WaypointSymbol, SubmittedBy = w.Chart.SubmittedBy, SubmittedOn = w.Chart.SubmittedOn },
            IsUnderConstruction = w.IsUnderConstruction,
        };
    }
}
