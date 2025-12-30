using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
        if (Surface == null)
            return;
        var shipyard = data.GetValueOrDefault(SystemSymbol).First(s => s.Symbol == WaypointSymbol);
        var ship = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        if (ship is null)
            return;
        ShipyardShip = ship;
        Title = $"{ShipyardShip.Type} {ShipyardShip.Frame.Name} at shipyard {shipyard.Symbol}";
        Binds["PurchasePrice"].SetData([$"{ShipyardShip.PurchasePrice}"]);
        Binds["Activity"].SetData([$"{ShipyardShip.Activity}"]);
        Binds["Supply"].SetData([$"{ShipyardShip.Supply}"]);
        Binds["Name"].SetData([$"{ShipyardShip.Name}"]);
        Binds["Shipyard"].SetData([$"Shipyard {WaypointSymbol} in {SystemSymbol}"]);
        Binds["Type"].SetData([$"{ShipyardShip.Type} {ShipyardShip.Frame.Name}"]);
        Binds["Navigation.Status"].SetData([$"Purchasable at {WaypointSymbol} in {SystemSymbol}"]);
        Binds["Navigation.WaypointSymbol"].SetData([$"{WaypointSymbol}"]);
        Binds["Cargo"].SetData([$"(0 / {ShipyardShip.CargoCapacity})"]);
        Binds["Fuel"].SetData([$"{ShipyardShip.Frame.FuelCapacity} / {ShipyardShip.Frame.FuelCapacity}"]);
        Binds["Cooldown"].SetData(["No Cooldown"]);
        Binds["Frame"].SetData([$"{ShipyardShip.Frame.Name}"]);
        Binds["Reactor"].SetData([$"{ShipyardShip.Reactor.Name} ({ShipyardShip.Reactor.PowerOutput} power)"]);
        Binds["Engine"].SetData([$"{ShipyardShip.Engine.Name} ({ShipyardShip.Engine.Speed} speed)"]);
        Binds["Modules"].SetData([$"Modules ({ShipyardShip.Modules.Count})"]);
        Binds["Mounts"].SetData([$"Mounts ({ShipyardShip.Mounts.Count})"]);
        Binds["Crew"].SetData([$"Crew ({ShipyardShip.Crew.Required} / {ShipyardShip.Crew.Capacity})"]);

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Price:", 2, y);
        Binds.Add("PurchasePrice", Controls.AddLabel($"Ship.PurchasePrice", 14, y++));
        Controls.AddLabel($"Activity:", 2, y);
        Binds.Add("Activity", Controls.AddLabel($"Ship.Activity", 14, y++));
        Controls.AddLabel($"Supply:", 2, y);
        Binds.Add("Supply", Controls.AddLabel($"Ship.Supply", 14, y++));
        y++;

        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Ship.Name", 14, y++));
        Controls.AddLabel($"Faction:", 2, y);
        Binds.Add("Shipyard", Controls.AddLabel($"Ship.Shipyard", 14, y++));
        Controls.AddLabel($"Role:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"Ship.Type", 14, y++));
        y++;
        Controls.AddLabel($"Navigation:", 2, y);
        Binds.Add("Navigation.Status", Controls.AddLabel($"Ship.Navigation.Status", 14, y++));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Navigation.WaypointSymbol", Controls.AddButton($"Ship.Navigation.WaypointSymbol", 14, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([WaypointSymbol, SystemSymbol])));
        Controls.AddLabel($"Cargo:", 2, y);
        Binds.Add("Cargo", Controls.AddLabel($"Ship.Cargo", 14, y++));
        Controls.AddLabel($"Fuel:", 2, y);
        Binds.Add("Fuel", Controls.AddLabel($"Ship.Fuel", 14, y++));
        Binds.Add("Cooldown", Controls.AddLabel($"Ship.Cooldown", 2, y++));
        y++;
        Binds.Add("Frame", Controls.AddButton($"Frame.Name", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow>([ShipyardShip.Frame.Symbol.ToString()])));
        Binds.Add("Reactor", Controls.AddButton($"Reactor.Name", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow>([ShipyardShip.Reactor.Symbol.ToString()])));
        Binds.Add("Engine", Controls.AddButton($"Engine.Name", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow>([ShipyardShip.Engine.Symbol.ToString()])));
        Binds.Add("Modules", Controls.AddButton($"Modules", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardModulesWindow>([ShipSymbol, WaypointSymbol, SystemSymbol])));
        Binds.Add("Mounts", Controls.AddButton($"Mounts", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardMountsWindow>([ShipSymbol, WaypointSymbol, SystemSymbol])));
        Binds.Add("Crew", Controls.AddLabel($"Crew", 4, y++));
    }
}
