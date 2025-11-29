using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class TransactionsWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private Shipyard? Shipyard { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public TransactionsWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string symbol, string? parentSymbol)
    {
        Symbol = symbol;
        ParentSymbol = parentSymbol ?? string.Empty;
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        var shipyard = data.GetValueOrDefault(ParentSymbol).First(s => s.Symbol == Symbol);

        Title = $"Shipyard {shipyard.Symbol}";
        Shipyard = shipyard;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Shipyard is null)
        {
            Controls.AddLabel($"Transactions loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach (var transaction in Shipyard.Transactions)
        {
            Controls.AddLabel($"{transaction.Timestamp:u} - {transaction.WaypointSymbol} - {transaction.ShipType} - {transaction.Price} - {transaction.AgentSymbol}", 2, y++);
        }
        ResizeAndRedraw();
    }
}
