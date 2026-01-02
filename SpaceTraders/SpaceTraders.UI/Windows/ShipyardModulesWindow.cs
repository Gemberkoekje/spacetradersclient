using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardModulesWindow : DataBoundWindowWithContext<ImmutableArray<Module>, ShipyardShipContext>
{
    private readonly ShipyardService _shipyardService;
    private CustomListBox? _modulesListBox;

    public ShipyardModulesWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Shipyard>>>(
            handler => shipyardService.Updated += handler,
            handler => shipyardService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override ImmutableArray<Module> FetchData()
    {
        var shipyards = _shipyardService.GetShipyards().GetValueOrDefault(Context.System);
        if (shipyards.IsDefault) return [];
        var shipyard = shipyards.FirstOrDefault(s => s.Symbol == Context.Waypoint);
        var ship = shipyard?.Ships.FirstOrDefault(s => s.Type == Context.ShipType);
        return ship?.Modules ?? [];
    }

    protected override void BindData(ImmutableArray<Module> data)
    {
        Title = $"Modules for ship {Context.ShipType} at {Context.Waypoint} shipyard";
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
            RootScreen.ShowWindow<ModuleWindow, ModuleContext>(new (module.Symbol));
        }
    }
}
