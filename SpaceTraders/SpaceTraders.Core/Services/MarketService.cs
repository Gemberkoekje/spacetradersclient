using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.MarketModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class MarketService(Client.SpaceTradersService service, WaypointService waypointService, ShipService shipService, AgentService agentService)
{
    private ImmutableDictionary<string, ImmutableList<Market>> Markets { get; set; } = [];

    public event Action<ImmutableDictionary<string, ImmutableList<Market>>>? Updated;

    public async Task Initialize()
    {
    }

    private void Update(ImmutableDictionary<string, ImmutableList<Market>> markets)
    {
        Markets = markets;
        Updated?.Invoke(markets);
    }

    public async Task AddWaypoint(string systemSymbol, string waypointSymbol)
    {
        if (!waypointService.GetWaypoints().GetValueOrDefault(systemSymbol)?.FirstOrDefault(wp => wp.Symbol == waypointSymbol)?.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace) ?? false)
            return;
        var shipyard = await service.EnqueueAsync((client, ct) => client.GetMarketAsync(systemSymbol, waypointSymbol, ct));
        var systemList = Markets.GetValueOrDefault(systemSymbol);
        if (systemList == null)
        {
            systemList = [];
        }
        systemList = systemList.Add(MapMarket(shipyard.Value.Data));
        Markets = Markets.SetItem(systemSymbol, systemList);
        Update(Markets);
    }

    public async Task<Result> Refuel(string shipSymbol)
    {
        var ship = shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));

        var location = Markets.GetValueOrDefault(ship.Navigation.SystemSymbol)?.FirstOrDefault(m => m.Symbol == ship.Navigation.WaypointSymbol);
        if (location == null)
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot be refueled, not at a market."));

        var result = await service.EnqueueAsync((client, ct) => client.RefuelShipAsync(new() { Units = ship.Fuel.Capacity - ship.Fuel.Current }, shipSymbol, ct), true);
        if (!result.IsValid)
            return result;
        agentService.Update(result.Value.Data.Agent);
        var endresult = await shipService.UpdateFuel(result.Value.Data.Fuel, ship.Symbol);
        if (!endresult.IsValid)
            return endresult;
        if (result.Value.Data.Cargo != null)
            return await shipService.UpdateCargo(result.Value.Data.Cargo, ship.Symbol);
        return endresult;
    }


    public ImmutableDictionary<string, ImmutableList<Market>> GetMarkets()
    {
        return Markets;
    }

    private static Market MapMarket(Client.Market w)
    {
        return new Market()
        {
            Symbol = w.Symbol,
            Exports = w.Exports.Select(tg => MapTradeGood(tg)).ToImmutableHashSet(),
            Imports = w.Imports.Select(tg => MapTradeGood(tg)).ToImmutableHashSet(),
            Exchange = w.Exchange.Select(tg => MapTradeGood(tg)).ToImmutableHashSet(),
            Transactions = w.Transactions.Select(t => new Transaction()
            {
                WaypointSymbol = t.WaypointSymbol,
                ShipSymbol = t.ShipSymbol,
                TradeSymbol = t.TradeSymbol,
                Type = t.Type.Convert<Client.MarketTransactionType, MarketTransactionType>(),
                Units = t.Units,
                PricePerUnit = t.PricePerUnit,
                TotalPrice = t.TotalPrice,
                Timestamp = t.Timestamp,
            }).ToImmutableHashSet(),
            TradeGoods = w.TradeGoods.Select(tg => new MarketTradeGood()
            {
                Symbol = tg.Symbol.Convert<Client.TradeSymbol, TradeSymbol>(),
                Type = tg.Type.Convert<Client.MarketTradeGoodType, MarketTradeGoodType>(),
                TradeVolume = tg.TradeVolume,
                Supply = tg.Supply.Convert<Client.SupplyLevel, SupplyLevel>(),
                Activity = tg.Activity.Convert<Client.ActivityLevel, ActivityLevel>(),
                PurchasePrice = tg.PurchasePrice,
                SellPrice = tg.SellPrice,
            }).ToImmutableHashSet(),
        };
    }

    private static TradeGood MapTradeGood(Client.TradeGood tg)
    {
        return new TradeGood()
        {
            Symbol = tg.Symbol.Convert<Client.TradeSymbol, TradeSymbol>(),
            Name = tg.Name,
            Description = tg.Description,
        };
    }
}
