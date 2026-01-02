using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing system data.
/// </summary>
/// <param name="service">The SpaceTraders API service.</param>
/// <param name="waypointService">The waypoint service.</param>
#pragma warning disable CS9113 // Parameter is unread - waypointService reserved for future use
public sealed class SystemService(Client.SpaceTradersService service, WaypointService waypointService)
#pragma warning restore CS9113
{
    private ImmutableArray<SystemWaypoint> Systems { get; set; } = [];

    /// <summary>
    /// Event raised when systems are updated.
    /// </summary>
    public event Action<ImmutableArray<SystemWaypoint>>? Updated;

    /// <summary>
    /// Initializes the system service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task Initialize()
    {
        // Not yet implemented
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a system by symbol.
    /// </summary>
    /// <param name="symbol">The system symbol.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddSystem(SystemSymbol symbol)
    {
        var systems = await service.EnqueueAsync((client, ct) => client.GetSystemAsync(symbol.ToString(), ct));
        Update(Systems.Add(MapSystemWaypoint(systems.Value.Data)));
    }

    /// <summary>
    /// Gets all systems.
    /// </summary>
    /// <returns>The systems.</returns>
    public ImmutableArray<SystemWaypoint> GetSystems()
    {
        return Systems;
    }

    private void Update(IEnumerable<SystemWaypoint> ships)
    {
        Systems = [.. ships];
        Updated?.Invoke(Systems);
    }

    private static SystemWaypoint MapSystemWaypoint(Client.StarSystem s)
    {
        return new SystemWaypoint()
        {
            Constellation = s.Constellation,
            Symbol = SystemSymbol.Parse(s.Symbol),
            SectorSymbol = SectorSymbol.Parse(s.SectorSymbol),
            SystemType = s.Type.Convert<Client.SystemType, SystemType>(),
            X = s.X,
            Y = s.Y,
            Factions = [.. s.Factions.Select(f => f.Symbol.Convert<Client.FactionSymbol, FactionSymbol>())],
            Name = s.Name,
        };
    }
}
