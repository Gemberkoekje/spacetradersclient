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
    Scheduler scheduler
    ) : BackgroundService
#pragma warning restore S107 // Methods should not have too many parameters
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await agentService.Initialize();
        contractService.Expired += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => contractService.Expire());
        await contractService.Initialize();
        shipService.Updated += LoadSystemsForShips;
        await shipService.Initialize();
        shipService.Arrived += (dateTimeOffset) => scheduler.Enqueue(dateTimeOffset, async () => await shipService.Arrive());
        await systemService.Initialize();
        await waypointService.Initialize();
        await shipyardService.Initialize();
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
        var shipyardSymbols = ships
            .Select(s => (s.Navigation.SystemSymbol, s.Navigation.WaypointSymbol))
            .Distinct()
            .ToArray();
        foreach (var shipyardSymbol in shipyardSymbols)
        {
            await shipyardService.AddWaypoint(shipyardSymbol.SystemSymbol, shipyardSymbol.WaypointSymbol);
        }
    }

    public void ScheduleCommand(Func<Task> command)
    {
        scheduler.Enqueue(Clock.UtcNow(), command);
    }

}
