using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.SystemModels;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing waypoint data.
/// </summary>
/// <param name="service">The SpaceTraders API service.</param>
public sealed class WaypointService(Client.SpaceTradersService service)
{
    private ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>> Waypoints { get; set; } = ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>>.Empty;

    /// <summary>
    /// Event raised when waypoints are updated.
    /// </summary>
    public event Func<ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>>, Task>? Updated;

    /// <summary>
    /// Initializes the waypoint service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task Initialize()
    {
        // Not yet implemented
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds all waypoints for a system.
    /// </summary>
    /// <param name="symbol">The system symbol.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddSystem(SystemSymbol symbol)
    {
        var systemWaypoints = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetSystemWaypointsAsync(page, limit, null, null, symbol.ToString(), ct),
            page => page.Data);
        var waypoints = systemWaypoints.Value.Select(w => MapWaypoint(w));
        var newDict = Waypoints.SetItem(symbol, [.. waypoints]);
        Update(newDict);
    }

    /// <summary>
    /// Gets all waypoints.
    /// </summary>
    /// <returns>The waypoints dictionary.</returns>
    public ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>> GetWaypoints()
    {
        return Waypoints;
    }

    private void Update(ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>> waypoints)
    {
        Waypoints = waypoints;
        Updated?.Invoke(waypoints);
    }

    private static Waypoint MapWaypoint(Client.Waypoint w)
    {
        return new Waypoint()
        {
            SystemSymbol = SystemSymbol.Parse(w.SystemSymbol),
            Symbol = WaypointSymbol.Parse(w.Symbol),
            Type = w.Type.Convert<Client.WaypointType, WaypointType>(),
            X = w.X,
            Y = w.Y,
            Orbitals = [.. w.Orbitals.Select(o => WaypointSymbol.Parse(o.Symbol))],
            Orbits = WaypointSymbol.Parse(w.Orbits),
            Traits = [.. w.Traits.Select(t => new WaypointTrait() { Name = t.Name, Description = t.Description, Symbol = t.Symbol.Convert<Client.WaypointTraitSymbol, Enums.WaypointTraitSymbol>() })],
            Modifiers = [.. w.Modifiers.Select(m => new WaypointModifier() { Name = m.Name, Description = m.Description, Symbol = m.Symbol.Convert<Client.WaypointModifierSymbol, WaypointModifierSymbol>() })],
            Chart = new Chart() { WaypointSymbol = WaypointSymbol.Parse(w.Chart.WaypointSymbol), SubmittedBy = AgentSymbol.Parse(w.Chart.SubmittedBy), SubmittedOn = w.Chart.SubmittedOn },
            IsUnderConstruction = w.IsUnderConstruction,
        };
    }
}
