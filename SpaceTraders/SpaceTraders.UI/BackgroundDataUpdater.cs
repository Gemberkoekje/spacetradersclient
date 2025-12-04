using Microsoft.Extensions.Hosting;
using Qowaiv;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SpaceTraders.UI;

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
    Scheduler scheduler
    ) : BackgroundService
#pragma warning restore S107 // Methods should not have too many parameters
{
    private DateTime LastUpdatedDateTime { get; set; }

    private DateTime LastShipUpdatedDateTime { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await agentService.Initialize();
        contractService.Expired += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => contractService.Expire());
        await contractService.Initialize();
        shipService.Updated += LoadSystemsForShips;
        await shipService.Initialize();
        shipService.Arrived += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => await shipService.Arrive());
        await systemService.Initialize();
        await marketService.Initialize();
        waypointService.Updated += LoadDetailsForSystems;
        await waypointService.Initialize();
        await shipyardService.Initialize();
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateClock();
            if (LastShipUpdatedDateTime.AddSeconds(120) < Clock.UtcNow())
            {
                LastShipUpdatedDateTime = Clock.UtcNow();
                scheduler.Enqueue(Clock.UtcNow(), async () => await UpdateSystemsForShips(shipService.GetShips().ToArray()));
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

    private async Task LoadSystemsForShips(Ship[] ships)
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

    private async Task UpdateSystemsForShips(Ship[] ships)
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
    private async Task LoadDetailsForSystems(ImmutableDictionary<string, ImmutableList<Waypoint>> waypoints)
    {
        foreach (var waypointset in waypoints)
        {
            foreach (var waypoint in waypointset.Value)
            {
                await shipyardService.AddWaypoint(waypointset.Key, waypoint.Symbol);
                await marketService.AddWaypoint(waypointset.Key, waypoint.Symbol);
            }
        }
    }

    public void ScheduleCommand(Func<Task> command)
    {
        scheduler.Enqueue(Clock.UtcNow(), command);
    }

}
