using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private Shipyard? Shipyard { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public ShipyardWindow(RootScreen rootScreen, ShipyardService shipyardService)
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
        var shipyard = data.GetValueOrDefault(ParentSymbol)?.FirstOrDefault(s => s.Symbol == Symbol);

        Title = $"Shipyard {Symbol} in {ParentSymbol}";
        Shipyard = shipyard;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Shipyard is null)
        {
            Controls.AddLabel($"No shipyard data.", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Symbol: {Shipyard.Symbol}", 2, y++);
        Controls.AddButton($"Transactions ({Shipyard.Transactions.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<TransactionsWindow>(Symbol, ParentSymbol));
        Controls.AddButton($"Ships ({Shipyard.Ships.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardShipsWindow>(Symbol, ParentSymbol));
        Controls.AddLabel($"Modifications fee: {Shipyard.ModificationsFee}", 2, y++);
        y++;
        Controls.AddLabel($"Ship Types:", 2, y++);
        foreach(var shipType in Shipyard.ShipTypes)
        {
            Controls.AddLabel($"- {shipType}", 4, y++);
        }
        ResizeAndRedraw();
    }
}
