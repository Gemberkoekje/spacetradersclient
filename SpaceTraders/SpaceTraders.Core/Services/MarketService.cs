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

/// <summary>
/// Service for managing market data.
/// </summary>
/// <param name="service">The SpaceTraders API service.</param>
/// <param name="waypointService">The waypoint service.</param>
/// <param name="shipService">The ship service.</param>
/// <param name="agentService">The agent service.</param>
public sealed class MarketService(Client.SpaceTradersService service, WaypointService waypointService, ShipService shipService, AgentService agentService)
{
    private ImmutableDictionary<string, ImmutableArray<Market>> Markets { get; set; } = ImmutableDictionary<string, ImmutableArray<Market>>.Empty;

    /// <summary>
    /// Event raised when markets are updated.
    /// </summary>
    public event Action<ImmutableDictionary<string, ImmutableArray<Market>>>? Updated;

    /// <summary>
    /// Initializes the market service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task Initialize()
    {
        // Not yet implemented
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a waypoint's market data.
    /// </summary>
    /// <param name="systemSymbol">The system symbol.</param>
    /// <param name="waypointSymbol">The waypoint symbol.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddWaypoint(string systemSymbol, string waypointSymbol)
    {
        var systemWaypoints = waypointService.GetWaypoints().GetValueOrDefault(systemSymbol);
        if (systemWaypoints.IsDefaultOrEmpty || (!systemWaypoints.FirstOrDefault(wp => wp.Symbol == waypointSymbol)?.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace) ?? false))
        {
            return;
        }

        var shipyard = await service.EnqueueAsync((client, ct) => client.GetMarketAsync(systemSymbol, waypointSymbol, ct));
        var systemList = Markets.GetValueOrDefault(systemSymbol);
        if (systemList.IsDefault)
        {
            systemList = [];
        }

        systemList = systemList.Add(MapMarket(shipyard.Value.Data));
        Markets = Markets.SetItem(systemSymbol, systemList);
        Update(Markets);
    }

    /// <summary>
    /// Refuels a ship.
    /// </summary>
    /// <param name="shipSymbol">The ship symbol.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> Refuel(string shipSymbol)
    {
        var ship = shipService.GetShips().FirstOrDefault(s => s.Symbol == shipSymbol);
        if (ship == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"No ship found with symbol {shipSymbol}."));
        }

        var systemMarkets = Markets.GetValueOrDefault(ship.Navigation.SystemSymbol);
        var location = systemMarkets.IsDefault ? null : systemMarkets.FirstOrDefault(m => m.Symbol == ship.Navigation.WaypointSymbol);
        if (location == null)
        {
            return Result.WithMessages(ValidationMessage.Error($"Ship cannot be refueled, not at a market."));
        }

        var result = await service.EnqueueAsync((client, ct) => client.RefuelShipAsync(new () { Units = ship.Fuel.Capacity - ship.Fuel.Current }, shipSymbol, ct), true);
        if (!result.IsValid)
        {
            return result;
        }

        agentService.Update(result.Value.Data.Agent);
        var endresult = await shipService.UpdateFuel(result.Value.Data.Fuel, ship.Symbol);
        if (!endresult.IsValid)
        {
            return endresult;
        }

        if (result.Value.Data.Cargo != null)
        {
            return await shipService.UpdateCargo(result.Value.Data.Cargo, ship.Symbol);
        }

        return endresult;
    }

    /// <summary>
    /// Gets all markets.
    /// </summary>
    /// <returns>The markets dictionary.</returns>
    public ImmutableDictionary<string, ImmutableArray<Market>> GetMarkets()
    {
        return Markets;
    }

    private void Update(ImmutableDictionary<string, ImmutableArray<Market>> markets)
    {
        Markets = markets;
        Updated?.Invoke(markets);
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
