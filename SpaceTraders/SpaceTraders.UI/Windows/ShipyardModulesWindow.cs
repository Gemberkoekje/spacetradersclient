using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardModulesWindow : ClosableWindow, ICanSetSymbols
{
    private string ShipSymbol { get; set; } = string.Empty;
    private string SystemSymbol { get; set; } = string.Empty;
    private string WaypointSymbol { get; set; } = string.Empty;

    private ImmutableList<Module> Modules { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public ShipyardModulesWindow(RootScreen rootScreen, ShipyardService shipyardService)
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
        var ship = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        Modules = ship?.Modules ?? [];
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Modules is null)
        {
            Controls.AddLabel($"Modules loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach(var module in Modules)
        {
            Controls.AddButton($"{module.Name} (Capacity: {module.Capacity}, Range: {module.Range})", 2, y++, (_, _) => RootScreen.ShowWindow<ModuleWindow>([module.Symbol.ToString()]));
        }

        ResizeAndRedraw();
    }
}
