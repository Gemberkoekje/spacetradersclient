using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ShipyardModels;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing shipyard data.
/// </summary>
/// <param name="service">The SpaceTraders API service.</param>
/// <param name="waypointService">The waypoint service.</param>
/// <param name="moduleService">The module service.</param>
public sealed class ShipyardService(Client.SpaceTradersService service, WaypointService waypointService, ModuleService moduleService)
{
    private ImmutableDictionary<string, ImmutableArray<Shipyard>> Shipyards { get; set; } = ImmutableDictionary<string, ImmutableArray<Shipyard>>.Empty;

    /// <summary>
    /// Event raised when shipyards are updated.
    /// </summary>
    public event Action<ImmutableDictionary<string, ImmutableArray<Shipyard>>>? Updated;

    /// <summary>
    /// Initializes the shipyard service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task Initialize()
    {
        // Not yet implemented
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a waypoint's shipyard data.
    /// </summary>
    /// <param name="systemSymbol">The system symbol.</param>
    /// <param name="waypointSymbol">The waypoint symbol.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddWaypoint(string systemSymbol, string waypointSymbol)
    {
        var waypoints = waypointService.GetWaypoints().GetValueOrDefault(systemSymbol);
        if (waypoints.IsDefault || (!waypoints.FirstOrDefault(wp => wp.Symbol == waypointSymbol)?.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard) ?? false))
        {
            return;
        }

        var shipyard = await service.EnqueueAsync((client, ct) => client.GetShipyardAsync(systemSymbol, waypointSymbol, ct));
        var systemList = Shipyards.GetValueOrDefault(systemSymbol);
        if (systemList.IsDefault)
        {
            systemList = [];
        }

        systemList = systemList.Add(MapShipyard(shipyard.Value.Data));
        Shipyards = Shipyards.SetItem(systemSymbol, systemList);
        moduleService.AddEngines([.. Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Engine)))]);
        moduleService.AddReactors([.. Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Reactor)))]);
        moduleService.AddFrames([.. Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Frame)))]);
        moduleService.AddModules([.. Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.SelectMany(t => t.Modules)))]);
        moduleService.AddMounts([.. Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.SelectMany(t => t.Mounts)))]);
        Update(Shipyards);
    }

    /// <summary>
    /// Gets all shipyards.
    /// </summary>
    /// <returns>The shipyards dictionary.</returns>
    public ImmutableDictionary<string, ImmutableArray<Shipyard>> GetShipyards()
    {
        return Shipyards;
    }

    private void Update(ImmutableDictionary<string, ImmutableArray<Shipyard>> shipyards)
    {
        Shipyards = shipyards;
        Updated?.Invoke(shipyards);
    }

    private static Shipyard MapShipyard(Client.Shipyard w)
    {
        return new Shipyard()
        {
            Symbol = w.Symbol,
            ShipTypes = w.ShipTypes.Select(st => st.Type.Convert<Client.ShipType, ShipType>()).ToImmutableHashSet(),
            Transactions = w.Transactions.Select(t => new Transaction()
            {
                WaypointSymbol = t.WaypointSymbol,
                ShipType = t.ShipType,
                Price = t.Price,
                AgentSymbol = t.AgentSymbol,
                Timestamp = t.Timestamp,
            }).ToImmutableHashSet(),
            Ships = w.Ships.Select(s => new ShipyardShip()
            {
                Type = s.Type.Convert<Client.ShipType, ShipType>(),
                Name = s.Name,
                Description = s.Description,
                Activity = s.Activity.Convert<Client.ActivityLevel, ActivityLevel>(),
                Supply = s.Supply.Convert<Client.SupplyLevel, SupplyLevel>(),
                PurchasePrice = s.PurchasePrice,
                Crew = new Crew()
                {
                    Capacity = s.Crew.Capacity,
                    Required = s.Crew.Required,
                },
                Frame = Mapper.MapFrame(s.Frame),
                Reactor = Mapper.MapReactor(s.Reactor),
                Engine = Mapper.MapEngine(s.Engine),
                Modules = Mapper.MapModules(s.Modules),
                Mounts = Mapper.MapMounts(s.Mounts),
            }).ToImmutableHashSet(),
            ModificationsFee = w.ModificationsFee,
        };
    }
}
