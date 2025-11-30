using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardShipWindow : ClosableWindow, ICanSetSymbols
{
    private string ShipSymbol { get; set; } = string.Empty;
    private string SystemSymbol { get; set; } = string.Empty;
    private string WaypointSymbol { get; set; } = string.Empty;

    private ShipyardShip? ShipyardShip { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public ShipyardShipWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        ShipSymbol = symbols[0];
        WaypointSymbol = symbols[1];
        SystemSymbol = symbols[2] ?? string.Empty;
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        var shipyard = data.GetValueOrDefault(SystemSymbol).First(s => s.Symbol == WaypointSymbol);

        Title = $"Shipyard {shipyard.Symbol}";
        ShipyardShip = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (ShipyardShip is null)
        {
            Controls.AddLabel($"Ships loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Type: {ShipyardShip.Type}", 2, y++);
        Controls.AddLabel($"Name: {ShipyardShip.Name}", 2, y++);
        Controls.AddLabel($"Activity: {ShipyardShip.Activity}", 2, y++);
        Controls.AddLabel($"Supply: {ShipyardShip.Supply}", 2, y++);
        Controls.AddLabel($"PurchasePrice: {ShipyardShip.PurchasePrice}", 2, y++);
        y++;
        Controls.AddLabel($"Cargo (0 / {ShipyardShip.CargoCapacity})", 2, y++);
        Controls.AddLabel($"Fuel: {ShipyardShip.Frame.FuelCapacity} / {ShipyardShip.Frame.FuelCapacity}", 2, y++);
        y++;
        Controls.AddButton($"{ShipyardShip.Frame.Name}", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow>([ShipyardShip.Frame.Symbol.ToString()]));
        Controls.AddButton($"{ShipyardShip.Reactor.Name} ({ShipyardShip.Reactor.PowerOutput} power)", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow>([ShipyardShip.Reactor.Symbol.ToString()]));
        Controls.AddButton($"{ShipyardShip.Engine.Name} ({ShipyardShip.Engine.Speed} speed)", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow>([ShipyardShip.Engine.Symbol.ToString()]));
        Controls.AddButton($"Modules ({ShipyardShip.Modules.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardModulesWindow>([ShipSymbol, WaypointSymbol, SystemSymbol]));
        Controls.AddButton($"Mounts ({ShipyardShip.Mounts.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardMountsWindow>([ShipSymbol, WaypointSymbol, SystemSymbol]));
        Controls.AddLabel($"Crew ({ShipyardShip.Crew.Required} / {ShipyardShip.Crew.Capacity})", 2, y++);
        ResizeAndRedraw();
    }
}
