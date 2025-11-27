using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public class SystemService(Client.SpaceTradersService service, WaypointService waypointService)
{
    public async Task<SystemWaypoint> GetSystemAsync(string symbol)
    {
        var response = await service.EnqueueCachedAsync((client, ct) => client.GetSystemAsync(symbol, ct), $"GetSystemAsync_{symbol}", TimeSpan.FromDays(1));
        var s = response.Value.Data;
        var systemWaypoint = new SystemWaypoint()
        {
            Constellation = s.Constellation,
            Symbol = s.Symbol,
            SectorSymbol = s.SectorSymbol,
            SystemType = s.Type.Convert(),
            X = s.X,
            Y = s.Y,
            Waypoints = await waypointService.GetWaypoints(symbol),
            Factions = [.. s.Factions.Select(f => f.Symbol.Convert())],
            Name = s.Name,
        };
        return systemWaypoint;
    }
}
