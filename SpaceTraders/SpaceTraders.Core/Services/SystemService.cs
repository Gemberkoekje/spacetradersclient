using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public class SystemService(Client.SpaceTradersService service)
{
    public SystemWaypoint GetSystem(string symbol)
    {
        return GetSystemAsync(symbol).GetAwaiter().GetResult();
    }

    public async Task<SystemWaypoint> GetSystemAsync(string symbol)
    {
        var response = await service.EnqueueCachedAsync((client, ct) => client.GetSystemAsync(symbol, ct), $"GetSystemAsync_{symbol}", TimeSpan.FromDays(1));
        var s = response.Data;
        var systemWaypoint = new SystemWaypoint()
        {
            Constellation = s.Constellation,
            Symbol = s.Symbol,
            SectorSymbol = s.SectorSymbol,
            SystemType = s.Type.Convert(),
            X = s.X,
            Y = s.Y,
            Waypoints = [.. s.Waypoints.Select(w => new Waypoint()
            {
                Symbol = w.Symbol,
                Type = w.Type.Convert(),
                X = w.X,
                Y = w.Y,
                Orbitals = w.Orbitals.Select(o => o.Symbol).ToList(),
                Orbits = w.Orbits,
                Traits = [],
                Modifiers = [],
                Chart = null,
                IsUnderConstruction = null,
            })],
            Factions = [.. s.Factions.Select(f => f.Symbol.Convert())],
            Name = s.Name,
        };
        return systemWaypoint;
    }
}
