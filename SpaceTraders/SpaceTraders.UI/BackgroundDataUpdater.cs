using Microsoft.Extensions.Hosting;
using Qowaiv;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

/// <summary>
/// Background service for updating data in the SpaceTraders application.
/// </summary>
/// <param name="rootScreen">The root screen.</param>
/// <param name="agentService">The agent service.</param>
/// <param name="contractService">The contract service.</param>
/// <param name="shipService">The ship service.</param>
/// <param name="systemService">The system service.</param>
/// <param name="waypointService">The waypoint service.</param>
/// <param name="shipyardService">The shipyard service.</param>
/// <param name="marketService">The market service.</param>
/// <param name="scheduler">The scheduler.</param>
#pragma warning disable S107 // Methods should not have too many parameters
public sealed class BackgroundDataUpdater(
    RootScreen rootScreen,
    AgentService agentService,
    ContractService contractService,
    ShipService shipService,
    SystemService systemService,
    WaypointService waypointService,
    ShipyardService shipyardService,
    MarketService marketService,
    Scheduler scheduler) : BackgroundService
#pragma warning restore S107 // Methods should not have too many parameters
{
    private DateTime LastUpdatedDateTime { get; set; }

    private DateTime LastShipUpdatedDateTime { get; set; }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await agentService.Initialize();
        contractService.Expired += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => contractService.Expire());
        await contractService.Initialize();
        shipService.Updated += LoadSystemsForShips;
        await shipService.Initialize();
        shipService.Arrived += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => await shipService.Arrive());
        await SystemService.Initialize();
        await MarketService.Initialize();
        waypointService.Updated += LoadDetailsForSystems;
        await WaypointService.Initialize();
        await ShipyardService.Initialize();
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateClock();
            if (LastShipUpdatedDateTime.AddSeconds(120) < Clock.UtcNow())
            {
                LastShipUpdatedDateTime = Clock.UtcNow();
                scheduler.Enqueue(Clock.UtcNow(), async () => await UpdateSystemsForShips([.. shipService.GetShips()]));
            }
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
        }
    }

    private async Task LoadSystemsForShips(ImmutableArray<Ship> ships)
    {
        var systemSymbols = ships
            .Select(s => s.Navigation.SystemSymbol)
            .Distinct()
            .ToArray();
        foreach (var systemSymbol in systemSymbols)
        {
            await systemService.AddSystem(systemSymbol);
            await waypointService.AddSystem(systemSymbol);
        }
        await UpdateSystemsForShips(ships);
    }

    private async Task UpdateSystemsForShips(ImmutableArray<Ship> ships)
    {
        var waypointSymbols = ships
            .Select(s => (s.Navigation.SystemSymbol, s.Navigation.WaypointSymbol))
            .Distinct()
            .ToArray();
        foreach (var shipyardSymbol in waypointSymbols)
        {
            await shipyardService.AddWaypoint(shipyardSymbol.SystemSymbol, shipyardSymbol.WaypointSymbol);
            await marketService.AddWaypoint(shipyardSymbol.SystemSymbol, shipyardSymbol.WaypointSymbol);
        }
    }

    private async Task UpdateClock()
    {
        if (LastUpdatedDateTime.Second == Clock.UtcNow().Second)
            return;
        rootScreen.UpdateClock(Clock.UtcNow());
        LastUpdatedDateTime = Clock.UtcNow();
    }

    private async Task LoadDetailsForSystems(ImmutableDictionary<string, ImmutableArray<Waypoint>> waypoints)
    {
        var waypointSymbols = waypoints
            .SelectMany(w => w.Value.Select(waypoint => (w.Key, waypoint.Symbol)));
        foreach (var (systemKey, waypointSymbol) in waypointSymbols)
        {
            await shipyardService.AddWaypoint(systemKey, waypointSymbol);
            await marketService.AddWaypoint(systemKey, waypointSymbol);
        }
    }

    /// <summary>
    /// Schedules a command to be executed.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    public void ScheduleCommand(Func<Task> command)
    {
        scheduler.Enqueue(Clock.UtcNow(), command);
    }
}
