using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardModulesWindow : DataBoundWindowWithSymbols<ImmutableArray<Module>>
{
    private readonly ShipyardService _shipyardService;
    private CustomListBox? _modulesListBox;

    // Symbols: [0] = ShipSymbol, [1] = WaypointSymbol, [2] = SystemSymbol
    private string ShipSymbol => Symbols.Length > 0 ? Symbols[0] : string.Empty;

    private string WaypointSymbol => Symbols.Length > 1 ? Symbols[1] : string.Empty;

    private string SystemSymbol => Symbols.Length > 2 ? Symbols[2] : string.Empty;

    public ShipyardModulesWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;

        SubscribeToEvent<ImmutableDictionary<string, ImmutableArray<Shipyard>>>(
            handler => shipyardService.Updated += handler,
            handler => shipyardService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override ImmutableArray<Module> FetchData()
    {
        var shipyards = _shipyardService.GetShipyards().GetValueOrDefault(SystemSymbol);
        if (shipyards.IsDefault) return [];
        var shipyard = shipyards.FirstOrDefault(s => s.Symbol == WaypointSymbol);
        var ship = shipyard?.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        return ship?.Modules ?? [];
    }

    protected override void BindData(ImmutableArray<Module> data)
    {
        Title = $"Shipyard {WaypointSymbol}";
        Binds["Modules"].SetData([.. data.Select(module => $"{module.Name} (Capacity: {module.Capacity}, Range: {module.Range})")]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        _modulesListBox = Controls.AddListbox($"Modules", 2, y, 80, 10);
        Binds.Add("Modules", _modulesListBox);
        y += 10;
        Controls.AddButton("Show Module", 2, y, (_, _) => OpenModule());
    }

    private void OpenModule()
    {
        if (_modulesListBox is null || CurrentData.IsDefault)
            return;
        var listbox = _modulesListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < CurrentData.Length)
        {
            var module = CurrentData[index];
            RootScreen.ShowWindow<ModuleWindow>([module.Symbol.ToString()]);
        }
    }
}
