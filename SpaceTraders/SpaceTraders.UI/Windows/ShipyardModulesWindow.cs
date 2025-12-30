using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
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

    private CustomListBox ModulesListBox { get; set; }

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
        if (Surface == null)
            return;
        var shipyard = data.GetValueOrDefault(SystemSymbol).First(s => s.Symbol == WaypointSymbol);

        Title = $"Shipyard {shipyard.Symbol}";
        var ship = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        Modules = ship?.Modules ?? [];

        Binds["Modules"].SetData([.. Modules.Select(module => $"{module.Name} (Capacity: {module.Capacity}, Range: {module.Range})")]);

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        ModulesListBox = Controls.AddListbox($"Modules", 2, y, 80, 10);
        Binds.Add("Modules", ModulesListBox);
        y += 10;
        Controls.AddButton("Show Module", 2, y++, (_, _) => OpenModule());
    }

    private void OpenModule()
    {
        var listbox = ModulesListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Modules.Count)
        {
            var module = Modules[index];
            RootScreen.ShowWindow<ModuleWindow>([module.Symbol.ToString()]);
        }
    }
}
