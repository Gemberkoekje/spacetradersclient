using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.MarketModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections;
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
        var market = data.GetValueOrDefault(ParentSymbol)?.FirstOrDefault(s => s.Symbol == Symbol);

        Title = $"Market {Symbol} in {ParentSymbol}";
        Market = market;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Market is null)
        {
            Controls.AddLabel($"No market data.", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        y = MarketDataTable(Market.TradeGoods, MarketTradeGoodType.Export,y);
        y++;
        y = MarketDataTable(Market.TradeGoods, MarketTradeGoodType.Import, y);
        y++;
        y = MarketDataTable(Market.TradeGoods, MarketTradeGoodType.Exchange, y);
        y++;
        Controls.AddLabel($"Transactions:", 2, y++);
        foreach (var transactions in Market.Transactions)
        {
            Controls.AddLabel($" - {transactions.WaypointSymbol} {transactions.ShipSymbol} {transactions.TradeSymbol} {transactions.Type} {transactions.Units} {transactions.PricePerUnit} {transactions.TotalPrice} {transactions.Timestamp}", 4, y++);
        }
        y++;
        ResizeAndRedraw();
    }

    private int MarketDataTable(IEnumerable<MarketTradeGood> tradeGoods, MarketTradeGoodType type, int y)
    {
        if (!tradeGoods.Any(t => t.Type == type))
            return y;
        var maxSymbolLength = Math.Max(tradeGoods.Max(t => $"{t.Symbol}".Length), $"Symbol".Length);
        var maxTradeVolumeLength = Math.Max(tradeGoods.Max(t => $"{t.TradeVolume:#,###}".Length), $"Amount".Length);
        var maxPurchasePriceLength = Math.Max(tradeGoods.Max(t => $"{t.PurchasePrice:#,###}".Length), $"To buy".Length);
        var maxSellPriceLength = Math.Max(tradeGoods.Max(t => $"{t.SellPrice:#,###}".Length), $"To sell".Length);
        var maxSupplyLength = Math.Max(tradeGoods.Max(t => $"{t.Supply}".Length), $"Supply".Length);
        var maxActivityLength = Math.Max(tradeGoods.Max(t => $"{t.Activity}".Length), $"Activity".Length);
        Controls.AddLabel($"{type}:", 2, y++);
        Controls.AddLabel(
            $"| {$"Symbol".PadRight(maxSymbolLength)} " +
            $"| {$"Amount".PadLeft(maxTradeVolumeLength)} " +
            $"| {$"To buy".PadLeft(maxPurchasePriceLength)} " +
            $"| {$"To sell".PadLeft(maxSellPriceLength)} " +
            $"| {$"Supply".PadRight(maxSupplyLength)} " +
            $"| {$"Activity".PadRight(maxActivityLength)} |", 4, y++, Color.Gray);
        foreach (var tradeGood in tradeGoods.Where(t => t.Type == type))
        {
            Controls.AddLabel(
                $"| {$"{tradeGood.Symbol}".PadRight(maxSymbolLength)} " +
                $"| {$"{tradeGood.TradeVolume:#,###}".PadLeft(maxTradeVolumeLength)} " +
                $"| {$"{tradeGood.PurchasePrice:#,###}".PadLeft(maxPurchasePriceLength)} " +
                $"| {$"{tradeGood.SellPrice:#,###}".PadLeft(maxSellPriceLength)} " +
                $"| {$"{tradeGood.Supply}".PadRight(maxSupplyLength)} " +
                $"| {$"{tradeGood.Activity}".PadRight(maxActivityLength)} |", 4, y++);
        }

        return y;
    }
}
