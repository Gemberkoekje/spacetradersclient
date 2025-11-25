using SpaceTraders.Core.Loaders;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Models.GameModels;

public sealed class GameSession
{
    private readonly AgentService _agentService;
    private readonly ShipService _shipService;
    private readonly SystemService _systemService;

    private Task<Agent>? _agentTask;
    private Task<Ship[]>? _shipsTask;
    private Task? _initTask;

    public bool IsInitializing => _initTask is { IsCompleted: false };
    public bool IsInitialized => _initTask is { IsCompletedSuccessfully: true };
    public bool IsFailed => _initTask is { IsFaulted: true } || _initTask is { IsCanceled: true };
    public Exception? InitializationError => _initTask?.Exception;

    // 0..1 basic progress (coarse: 0, 0.5, 1)
    public double Progress
    {
        get
        {
            if (IsInitialized) return 1d;
            var count = 0;
            var done = 0;
            if (_agentTask != null) { count++; if (_agentTask.IsCompleted) done++; }
            if (_shipsTask != null) { count++; if (_shipsTask.IsCompleted) done++; }
            return count == 0 ? 0d : (double)done / count;
        }
    }

    public GameSession(AgentService agentService, ShipService shipService, SystemService systemService)
    {
        _agentService = agentService;
        _shipService = shipService;
        _systemService = systemService;
    }

    // Kick off loading without blocking UI.
    public void BeginInitialization(CancellationToken cancellationToken = default)
    {
        if (_initTask != null) return;

    }

    public Agent Agent =>
        _agentService.GetAgent().GetAwaiter().GetResult();

    public ReadOnlySpan<Ship> Ships =>
        _shipService.GetMyShips().GetAwaiter().GetResult();

    public SystemWaypoint GetSystem(string systemSymbol) =>
        _systemService.GetSystem(systemSymbol);
}