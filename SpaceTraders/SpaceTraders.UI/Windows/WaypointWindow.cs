using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.MarketModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class WaypointWindow : DataBoundWindowWithContext<Waypoint, WaypointContext>
{
    private readonly WaypointService _waypointService;
    private readonly ShipService _shipService;

    private ImmutableArray<Ship> _ships = [];
    private readonly Dictionary<string, CustomButton> _buttons = [];
    private CustomListBox? _shipsListBox;
    private CustomListBox? _orbitalsListBox;

    public WaypointWindow(RootScreen rootScreen, WaypointService waypointService, ShipService shipService)
        : base(rootScreen, 45, 30)
    {
        _waypointService = waypointService;
        _shipService = shipService;

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>>>(
            handler => waypointService.Updated += handler,
            handler => waypointService.Updated -= handler,
            OnServiceUpdated);

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnShipsUpdated);

        Initialize();
    }

    protected override Waypoint? FetchData()
    {
        var waypoints = _waypointService.GetWaypoints().GetValueOrDefault(Context.System);
        return waypoints.IsDefault ? null : waypoints.FirstOrDefault(d => d.Symbol == Context.Waypoint);
    }

    protected override void BindData(Waypoint data)
    {
        Title = $"Waypoint {Context.Waypoint} in {Context.System}";

        // Update ships for this waypoint
        _ships = [.. _shipService.GetShips().Where(d => d.Navigation.WaypointSymbol == data.Symbol)];

        Binds["Symbol"].SetData([$"{data.Symbol}"]);
        Binds["Type"].SetData([$"{data.Type}"]);
        Binds["Location"].SetData([$"{data.X}, {data.Y}"]);
        _buttons["Shipyard"].IsEnabled = data.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard);
        _buttons["Marketplace"].IsEnabled = data.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace);
        _buttons["Construction"].IsEnabled = false;
        _shipsListBox?.SetData([.. _ships.OrderBy(s => s.Symbol).Select(s => $"{s.Symbol} ({s.Registration.Role})")]);
        _orbitalsListBox?.SetData([.. data.Orbitals.OrderBy(w => w)]);
        _buttons["Orbits"].SetData([$"{(data.Orbits != WaypointSymbol.Empty ? data.Orbits : "Orbits the sun")}"]);
        _buttons["Orbits"].IsEnabled = data.Orbits != WaypointSymbol.Empty;
        Binds["Traits"].SetData([.. data.Traits.Where(t => !IsSpecialTrait(t.Symbol)).OrderBy(w => w.Name).Select(t => t.Name)]);
        Binds["Modifiers"].SetData([.. data.Modifiers.Select(m => m.Name)]);
    }

    private Task OnShipsUpdated(ImmutableArray<Ship> _)
    {
        // Ships changed, refresh to update the ships list
        RefreshData();
        return Task.CompletedTask;
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"Waypoint.Symbol", 21, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"Waypoint.Type", 21, y++));
        Controls.AddLabel($"Location:", 2, y);
        Binds.Add("Location", Controls.AddLabel($"Waypoint.Location", 21, y++));
        _buttons.Add("Shipyard", Controls.AddButton($"Shipyard", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardWindow, WaypointContext>(Context)));
        _buttons.Add("Marketplace", Controls.AddButton($"Marketplace", 2, y++, (_, _) => RootScreen.ShowWindow<MarketWindow, WaypointContext>(Context)));
        _buttons.Add("Construction", Controls.AddButton($"Is Under Construction", 2, y++, (_, _) => throw new NotSupportedException()));
        y++;
        Controls.AddLabel($"Ships at this Waypoint:", 2, y++);
        _shipsListBox = Controls.AddListbox($"Ships", 2, y, 80, 5);
        Binds.Add("Ships", _shipsListBox);
        y += 5;
        Controls.AddButton("Show Ship", 2, y++, (_, _) => OpenShip());
        y++;
        Controls.AddLabel($"Orbitals:", 2, y++);
        _orbitalsListBox = Controls.AddListbox($"Orbital", 2, y, 80, 7);
        Binds.Add("Orbitals", _orbitalsListBox);
        y += 7;
        Controls.AddButton("Show Orbital", 2, y++, (_, _) => OpenOrbital());
        y++;
        Controls.AddLabel($"Orbits:", 2, y);
        _buttons.Add("Orbits", Controls.AddButton($"Waypoint.Orbits", 11, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow, WaypointContext>(new (CurrentData!.Orbits, CurrentData.SystemSymbol))));
        y++;
        Controls.AddLabel($"Traits:", 2, y++);
        Binds.Add("Traits", Controls.AddListbox($"Traits", 2, y, 80, 7));
        y += 7;
        Controls.AddLabel($"Modifiers:", 2, y++);
        Binds.Add("Modifiers", Controls.AddListbox($"Modifiers", 2, y, 80, 7));
    }

    private static bool IsSpecialTrait(WaypointTraitSymbol trait)
    {
        return trait is WaypointTraitSymbol.Marketplace or
               WaypointTraitSymbol.Shipyard;
    }

    private void OpenShip()
    {
        if (_shipsListBox is null)
            return;
        var listbox = _shipsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < _ships.Length)
        {
            var ship = _ships[index];
            RootScreen.ShowWindow<ShipWindow, ShipContext>(new (ship.Symbol));
        }
    }

    private void OpenOrbital()
    {
        if (_orbitalsListBox is null || CurrentData is null)
            return;
        var listbox = _orbitalsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < CurrentData.Orbitals.Length)
        {
            var orbital = CurrentData.Orbitals[index];
            RootScreen.ShowWindow<WaypointWindow, WaypointContext>(new (orbital, CurrentData.SystemSymbol));
        }
    }
}
