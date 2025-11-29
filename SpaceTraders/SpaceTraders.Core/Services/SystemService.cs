using Qowaiv;
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

public class SystemService(Client.SpaceTradersService service, WaypointService waypointService)
{
    private ImmutableList<SystemWaypoint> Systems { get; set; } = [];

    public event Action<SystemWaypoint[]>? Updated;

    public async Task Initialize()
    {
    }

    private void Update(IEnumerable<SystemWaypoint> ships)
    {
        Systems = ships.ToImmutableList();
        Updated?.Invoke(ships.ToArray());
    }

    public async Task AddSystem(string symbol)
    {
        var systems = await service.EnqueueAsync((client, ct) => client.GetSystemAsync(symbol, ct));
        Update(Systems.Add(MapSystemWaypoint(systems.Value.Data)));
    }

    public ImmutableList<SystemWaypoint> GetSystems()
    {
        return Systems;
    }

    private static SystemWaypoint MapSystemWaypoint(Client.StarSystem s)
    {
        return new SystemWaypoint()
        {
            Constellation = s.Constellation,
            Symbol = s.Symbol,
            SectorSymbol = s.SectorSymbol,
            SystemType = s.Type.Convert<Client.SystemType, SystemType>(),
            X = s.X,
            Y = s.Y,
            Factions = [.. s.Factions.Select(f => f.Symbol.Convert<Client.FactionSymbol, FactionSymbol>())],
            Name = s.Name,
        };
    }
}
