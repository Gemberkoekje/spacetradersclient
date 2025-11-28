using Microsoft.Extensions.Hosting;
using SpaceTraders.Core.Loaders;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

public sealed class BackgroundDataUpdater(
    RootScreen rootScreen,
    AgentService agentService,
    ContractService contractService,
    ShipService shipService,
    SystemService systemService,
    WaypointService waypointService
    ) : BackgroundService
{
    private Func<string?, string?, Task<object>> GetAction(Type type)
    {
        switch (type)
        {
            case Type t when t == typeof(Agent):
                return async (_, _) => await agentService.GetAgent();
            case Type t when t == typeof(Contract[]):
                return async (_, _) => await contractService.GetMyContracts();
            case Type t when t == typeof(Ship):
                return async (value, _) => await shipService.GetShip(value!);
            case Type t when t == typeof(Navigation):
                return async (value, _) => (await shipService.GetShip(value!)).Navigation;
            case Type t when t == typeof(Ship[]):
                return async (_, _) => await shipService.GetMyShips();
            case Type t when t == typeof(SystemWaypoint):
                return async (value, _) => await systemService.GetSystemAsync(value!);
            case Type t when t == typeof(Waypoint):
                return async (value, parent) => await waypointService.GetWaypoint(parent!, value!);
            default:
                throw new NotSupportedException($"Type {type} is not supported for data loading.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            for (int x = 0; x < 2; x++)
            {
                foreach (var child in rootScreen.Windows.Where(c => c is ICanLoadData).ToList())
                {
                    var dataloadchild = child as ICanLoadData;
                    var ships = await GetAction(dataloadchild!.DataType)(dataloadchild.Symbol, dataloadchild.ParentSymbol);
                    dataloadchild.LoadData(ships);
                }
            }
            while (actions.TryDequeue(out var action))
            {
                await action();
            }
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Ignore cancellation exceptions
            }
        }
    }

    Queue<Func<Task>> actions = [];

    public void DoAsynchronousEventually(Func<Task> action)
    {
        actions.Enqueue(action);
    }

}
