using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.MarketModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class MarketWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;

    private string Symbol { get; set; } = string.Empty;

    private Market? Market { get; set; }

    private MarketService MarketService { get; init; }

    public MarketWindow(RootScreen rootScreen, MarketService marketService)
        : base(rootScreen, 52, 20)
    {
        MarketService = marketService;
        marketService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        ParentSymbol = symbols[1];
        LoadData(MarketService.GetMarkets());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Market>> data)
    {
        if (Surface == null)
            return;
        var market = data.GetValueOrDefault(ParentSymbol)?.FirstOrDefault(s => s.Symbol == Symbol);
        if (market is null)
            return;

        Title = $"Market {Symbol} in {ParentSymbol}";
        Market = market;

        BindMarketData("TradeGoodsExport", Market.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Export));
        BindMarketData("TradeGoodsImport", Market.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Import));
        BindMarketData("TradeGoodsExchange", Market.TradeGoods.Where(tg => tg.Type == MarketTradeGoodType.Exchange));
        Binds["Transactions"].SetData([.. Market.Transactions.OrderByDescending(t => t.Timestamp).Select(t => $"{t.Timestamp}: {t.ShipSymbol} {t.Type} {t.Units:#,###} of {t.TradeSymbol} at {t.PricePerUnit:#,###} credits/unit; total: {t.TotalPrice:#,###}")]);

        ResizeAndRedraw();
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

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Exports:", 2, y++);
        Binds.Add("TradeGoodsExport",   Controls.AddListbox($"TradeGoodsExport", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Imports:", 2, y++);
        Binds.Add("TradeGoodsImport",   Controls.AddListbox($"TradeGoodsImport", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Exchanges:", 2, y++);
        Binds.Add("TradeGoodsExchange", Controls.AddListbox($"TradeGoodsExchange", 2, y, 120, 10, false));
        y += 10;
        Controls.AddLabel($"Transactions:", 2, y++);
        Binds.Add("Transactions", Controls.AddListbox($"Transactions", 2, y, 120, 10, false));
        y++;
    }
}
