using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
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
    private ImmutableDictionary<string, ImmutableArray<Waypoint>> Waypoints { get; set; } = ImmutableDictionary<string, ImmutableArray<Waypoint>>.Empty;

    /// <summary>
    /// Event raised when waypoints are updated.
    /// </summary>
    public event Func<ImmutableDictionary<string, ImmutableArray<Waypoint>>, Task>? Updated;

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
    public async Task AddSystem(string symbol)
    {
        var systemWaypoints = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetSystemWaypointsAsync(page, limit, null, null, symbol, ct),
            page => page.Data);
        var waypoints = systemWaypoints.Value.Select(w => MapWaypoint(w));
        var newDict = Waypoints.SetItem(symbol, [.. waypoints]);
        Update(newDict);
    }

    /// <summary>
    /// Gets all waypoints.
    /// </summary>
    /// <returns>The waypoints dictionary.</returns>
    public ImmutableDictionary<string, ImmutableArray<Waypoint>> GetWaypoints()
    {
        return Waypoints;
    }

    private void Update(ImmutableDictionary<string, ImmutableArray<Waypoint>> waypoints)
    {
        Waypoints = waypoints;
        Updated?.Invoke(waypoints);
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
            Orbitals = [.. w.Orbitals.Select(o => o.Symbol)],
            Orbits = w.Orbits,
            Traits = [.. w.Traits.Select(t => new WaypointTrait() { Name = t.Name, Description = t.Description, Symbol = t.Symbol.Convert<Client.WaypointTraitSymbol, Enums.WaypointTraitSymbol>() })],
            Modifiers = [.. w.Modifiers.Select(m => new WaypointModifier() { Name = m.Name, Description = m.Description, Symbol = m.Symbol.Convert<Client.WaypointModifierSymbol, WaypointModifierSymbol>() })],
            Chart = new Chart() { WaypointSymbol = w.Chart.WaypointSymbol, SubmittedBy = w.Chart.SubmittedBy, SubmittedOn = w.Chart.SubmittedOn },
            IsUnderConstruction = w.IsUnderConstruction,
        };
    }
}
