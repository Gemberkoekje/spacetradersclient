using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class WaypointService(Client.SpaceTradersService service)
{
    private List<Waypoint> Waypoints = [];

    public async Task<Waypoint[]> GetWaypoints(string systemSymbol)
    {
        if(Waypoints.Count > 0)
        {
            return Waypoints.ToArray();
        }

        var systemWaypoints = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetSystemWaypointsAsync(page, limit, null, null, systemSymbol, ct),
            page => page.Data,
            $"GetWaypoints_null_null_{systemSymbol}",
            TimeSpan.FromDays(1));
        Waypoints = [.. systemWaypoints.Value.Select(w => MapWaypoint(w))];
        return Waypoints.ToArray();
    }

    public async Task<Waypoint> GetWaypoint(string systemSymbol, string symbol)
    {
        if (Waypoints.FirstOrDefault(w => w.SystemSymbol == systemSymbol && w.Symbol == symbol) is Waypoint waypoint)
        {
            return waypoint;
        }

        var response = await service.EnqueueCachedAsync((client, ct) => client.GetWaypointAsync(systemSymbol, symbol, ct), $"GetWaypointAsync_{systemSymbol}_{symbol}", TimeSpan.FromDays(1));
        var newwaypoint = MapWaypoint(response.Value.Data);
        Waypoints.Add(newwaypoint);
        return newwaypoint;
    }

    private static Waypoint MapWaypoint(Client.Waypoint w)
    {
        return new Waypoint()
        {
            SystemSymbol = w.SystemSymbol,
            Symbol = w.Symbol,
            Type = w.Type.Convert(),
            X = w.X,
            Y = w.Y,
            Orbitals = w.Orbitals.Select(o => o.Symbol).ToList(),
            Orbits = w.Orbits,
            Traits = [.. w.Traits.Select(t => new WaypointTrait() { Name = t.Name, Description = t.Description, Symbol = t.Symbol.Convert() })],
            Modifiers = [.. w.Modifiers.Select(m => new WaypointModifier() { Name = m.Name, Description = m.Description, Symbol = m.Symbol.Convert() })],
            Chart = new Chart() { WaypointSymbol = w.Chart.WaypointSymbol, SubmittedBy = w.Chart.SubmittedBy, SubmittedOn = w.Chart.SubmittedOn },
            IsUnderConstruction = w.IsUnderConstruction,
        };
    }
}
