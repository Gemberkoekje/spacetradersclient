using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ShipyardModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class ShipyardService(Client.SpaceTradersService service, WaypointService waypointService, ModuleService moduleService)
{
    private ImmutableDictionary<string, ImmutableList<Shipyard>> Shipyards { get; set; } = [];

    public event Action<ImmutableDictionary<string, ImmutableList<Shipyard>>>? Updated;

    public async Task Initialize()
    {
    }

    private void Update(ImmutableDictionary<string, ImmutableList<Shipyard>> shipyards)
    {
        Shipyards = shipyards;
        Updated?.Invoke(shipyards);
    }

    public async Task AddWaypoint(string systemSymbol, string waypointSymbol)
    {
        if (!waypointService.GetWaypoints().GetValueOrDefault(systemSymbol)?.FirstOrDefault(wp => wp.Symbol == waypointSymbol)?.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard) ?? false)
            return;
        var shipyard = await service.EnqueueAsync((client, ct) => client.GetShipyardAsync(systemSymbol, waypointSymbol, ct));
        var systemList = Shipyards.GetValueOrDefault(systemSymbol);
        if (systemList == null)
        {
            systemList = [];
        }
        systemList = systemList.Add(MapShipyard(shipyard.Value.Data));
        Shipyards = Shipyards.SetItem(systemSymbol, systemList);
        moduleService.AddEngines(Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Engine))).ToImmutableArray());
        moduleService.AddReactors(Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Reactor))).ToImmutableArray());
        moduleService.AddFrames(Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.Select(t => t.Frame))).ToImmutableArray());
        moduleService.AddModules(Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.SelectMany(t => t.Modules))).ToImmutableArray());
        moduleService.AddMounts(Shipyards.SelectMany(sy => sy.Value.SelectMany(s => s.Ships.SelectMany(t => t.Mounts))).ToImmutableArray());
        Update(Shipyards);
    }

    public ImmutableDictionary<string, ImmutableList<Shipyard>> GetShipyards()
    {
        return Shipyards;
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
