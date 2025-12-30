using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.MarketModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class MarketWindow : DataBoundWindowWithSymbols<Market>
{
    private readonly MarketService _marketService;

    public MarketWindow(RootScreen rootScreen, MarketService marketService)
        : base(rootScreen, 52, 20)
    {
        _marketService = marketService;

        SubscribeToEvent<ImmutableDictionary<string, ImmutableArray<Market>>>(
            handler => marketService.Updated += handler,
            handler => marketService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override Market? FetchData()
    {
        var markets = _marketService.GetMarkets().GetValueOrDefault(ParentSymbol);
        return markets.IsDefault ? null : markets.FirstOrDefault(s => s.Symbol == Symbol);
    }

    protected override void BindData(Market data)
    {
        Title = $"Market {Symbol} in {ParentSymbol}";

        BindMarketData("TradeGoodsExport", data.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Export));
        BindMarketData("TradeGoodsImport", data.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Import));
        BindMarketData("TradeGoodsExchange", data.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Exchange));
        Binds["Transactions"].SetData([.. data.Transactions.OrderByDescending(t => t.Timestamp).Select(t => $"{t.Timestamp}: {t.ShipSymbol} {t.Type} {t.Units:#,###} of {t.TradeSymbol} at {t.PricePerUnit:#,###} credits/unit; total: {t.TotalPrice:#,###}")]);
    }

    private void BindMarketData(string bindname, IEnumerable<MarketTradeGood> market)
    {
        if (!market.Any())
        {
            return;
        }
        var maxSymbolLength = Math.Max(market.Max(t => $"{t.Symbol}".Length), $"Symbol".Length);
        var maxTradeVolumeLength = Math.Max(market.Max(t => $"{t.TradeVolume:#,###}".Length), $"Amount".Length);
        var maxPurchasePriceLength = Math.Max(market.Max(t => $"{t.PurchasePrice:#,###}".Length), $"To buy".Length);
        var maxSellPriceLength = Math.Max(market.Max(t => $"{t.SellPrice:#,###}".Length), $"To sell".Length);
        var maxSupplyLength = Math.Max(market.Max(t => $"{t.Supply}".Length), $"Supply".Length);
        var maxActivityLength = Math.Max(market.Max(t => $"{t.Activity}".Length), $"Activity".Length);
        Binds[bindname].SetData([.. market.Select(tradeGood => $"{$"{tradeGood.Symbol}".PadRight(maxSymbolLength)} " +
                $"| {$"{tradeGood.TradeVolume:#,###}".PadLeft(maxTradeVolumeLength)} " +
                $"| {$"{tradeGood.PurchasePrice:#,###}".PadLeft(maxPurchasePriceLength)} " +
                $"| {$"{tradeGood.SellPrice:#,###}".PadLeft(maxSellPriceLength)} " +
                $"| {$"{tradeGood.Supply}".PadRight(maxSupplyLength)} " +
                $"| {$"{tradeGood.Activity}".PadRight(maxActivityLength)}")]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Exports:", 2, y++);
        Binds.Add("TradeGoodsExport", Controls.AddListbox($"TradeGoodsExport", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Imports:", 2, y++);
        Binds.Add("TradeGoodsImport", Controls.AddListbox($"TradeGoodsImport", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Exchanges:", 2, y++);
        Binds.Add("TradeGoodsExchange", Controls.AddListbox($"TradeGoodsExchange", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Transactions:", 2, y++);
        Binds.Add("Transactions", Controls.AddListbox($"Transactions", 2, y, 120, 10, false));
    }
}
