using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardShipsWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private Shipyard? Shipyard { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public ShipyardShipsWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        ParentSymbol = symbols[1] ?? string.Empty;
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        if (Surface == null)
            return;
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
            Controls.AddLabel($"Ships loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach (var ship in Shipyard.Ships)
        {
            Controls.AddLabel($"Type: {ship.Type}", 2, y++);
            Controls.AddLabel($"Name: {ship.Name}", 2, y++);
            Controls.AddLabel($"Activity: {ship.Activity}", 2, y++);
            Controls.AddLabel($"Supply: {ship.Supply}", 2, y++);
            Controls.AddLabel($"PurchasePrice: {ship.PurchasePrice}", 2, y++);
            Controls.AddLabel($"Crew required: {ship.Crew.Required}", 2, y++);
            Controls.AddLabel($"Crew capacity: {ship.Crew.Capacity}", 2, y++);
            y++;
            Controls.AddLabel($"{ship.Frame.Name}", 2, y++);
            Controls.AddLabel($"{ship.Reactor.Name} ({ship.Reactor.PowerOutput} power)", 2, y++);
            Controls.AddLabel($"{ship.Engine.Name} ({ship.Engine.Speed} speed)", 2, y++);
            Controls.AddLabel($"Modules ({ship.Modules.Count})", 2, y++);
            foreach (var module in ship.Modules)
            {
                Controls.AddLabel($"- {module.Name}", 4, y++);
            }
            Controls.AddLabel($"Mounts ({ship.Mounts.Count})", 2, y++);
            foreach (var mount in ship.Mounts)
            {
                Controls.AddLabel($"- {mount.Name}", 4, y++);
            }
            y++;
            Controls.AddLabel($"---", 2, y++);
            y++;
        }
        ResizeAndRedraw();
    }
}
