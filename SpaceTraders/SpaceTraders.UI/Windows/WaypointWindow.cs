using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class WaypointWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private string ParentSymbol { get; set; } = string.Empty;

    private Waypoint? Waypoint { get; set; }

    private Ship[] Ships { get; set; } = [];

    private Dictionary<string, CustomButton> Buttons { get; set; } = [];
    private CustomListBox? ShipsListBox { get; set; }
    private CustomListBox? OrbitalsListBox { get; set; }

    private WaypointService WaypointService { get; init; }
    private ShipService ShipService { get; init; }

    public WaypointWindow(RootScreen rootScreen, WaypointService waypointService, ShipService shipService)
        : base(rootScreen, 45, 30)
    {
        waypointService.Updated += LoadData;
        shipService.Updated += LoadData;
        WaypointService = waypointService;
        ShipService = shipService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        ParentSymbol = symbols[1];
        LoadData(WaypointService.GetWaypoints());
        LoadData(ShipService.GetShips().ToArray());
    }

    public Task LoadData(ImmutableDictionary<string, ImmutableList<Waypoint>> data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var waypoints = data.GetValueOrDefault(ParentSymbol);
        var waypoint = waypoints?.FirstOrDefault(d => d.Symbol == Symbol);
        if (Waypoint is not null && Waypoint == waypoint)
            return Task.CompletedTask;

        Title = $"Waypoint {Symbol} in {ParentSymbol}";
        Waypoint = waypoint;




        RebindAndResize();
        return Task.CompletedTask;
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var relevantData = data.Where(d => d.Navigation.WaypointSymbol == Waypoint?.Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;
        Ships = relevantData;




        RebindAndResize();
        return Task.CompletedTask;
    }

    private void RebindAndResize()
    {
        if (Waypoint is null)
            return;
        Binds["Symbol"].SetData([$"{Waypoint.Symbol}"]);
        Binds["Type"].SetData([$"{Waypoint.Type}"]);
        Binds["Location"].SetData([$"{Waypoint.X}, {Waypoint.Y}"]);
        Buttons["Shipyard"].IsEnabled = Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard);
        Buttons["Marketplace"].IsEnabled = Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace);
        Buttons["Construction"].IsEnabled = false;
        ShipsListBox.SetData([.. Ships.OrderBy(s => s.Symbol).Select(s => $"{s.Symbol} ({s.Registration.Role})")]);
        OrbitalsListBox.SetData([.. Waypoint.Orbitals.OrderBy(w => w)]);
        Buttons["Orbits"].SetData([$"{(!string.IsNullOrEmpty(Waypoint.Orbits) ? Waypoint.Orbits : "Orbits the sun")}"]);
        Buttons["Orbits"].IsEnabled = !string.IsNullOrEmpty(Waypoint.Orbits);
        Binds["Traits"].SetData([.. Waypoint.Traits.Where(t => !IsSpecialTrait(t.Symbol)).OrderBy(w => w.Name).Select(t => t.Name)]);
        Binds["Modifiers"].SetData([.. Waypoint.Modifiers.Select(m => m.Name)]);
        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"Waypoint.Symbol", 21, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"Waypoint.Type", 21, y++));
        Controls.AddLabel($"Location:", 2, y);
        Binds.Add("Location", Controls.AddLabel($"Waypoint.Location", 21, y++));
        Buttons.Add("Shipyard", Controls.AddButton($"Shipyard", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardWindow>([Symbol, ParentSymbol])));
        Buttons.Add("Marketplace", Controls.AddButton($"Marketplace", 2, y++, (_, _) => RootScreen.ShowWindow<MarketWindow>([Symbol, ParentSymbol])));
        Buttons.Add("Construction", Controls.AddButton($"Is Under Construction", 2, y++, (_, _) => throw new NotSupportedException()));
        y++;
        Controls.AddLabel($"Ships at this Waypoint:", 2, y++);
        ShipsListBox = Controls.AddListbox($"Ships", 2, y, 80, 5);
        Binds.Add("Ships", ShipsListBox);
        y += 5;
        Controls.AddButton("Show Ship", 2, y++, (_, _) => OpenShip());
        y++;
        Controls.AddLabel($"Orbitals:", 2, y++);
        OrbitalsListBox = Controls.AddListbox($"Orbital", 2, y, 80, 7);
        Binds.Add("Orbitals", OrbitalsListBox);
        y += 7;
        Controls.AddButton("Show Orbital", 2, y++, (_, _) => OpenOrbital());
        y++;
        Controls.AddLabel($"Orbits:", 2, y);
        Buttons.Add("Orbits", Controls.AddButton($"Waypoint.Orbits", 11, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([Waypoint.Orbits, Waypoint.SystemSymbol])));
        y++;
        Controls.AddLabel($"Traits:", 2, y++);
        Binds.Add("Traits", Controls.AddListbox($"Traits", 2, y, 80, 7));
        y += 7;
        Controls.AddLabel($"Modifiers:", 2, y++);
        Binds.Add("Modifiers", Controls.AddListbox($"Modifiers", 2, y, 80, 7));
        y += 7;
    }

    private bool IsSpecialTrait(WaypointTraitSymbol trait)
    {
        return trait is WaypointTraitSymbol.Marketplace or
               WaypointTraitSymbol.Shipyard;
    }

    private void OpenShip()
    {
        if (ShipsListBox is null)
            return;
        var listbox = ShipsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Ships.Length)
        {
            var ship = Ships[index];
            RootScreen.ShowWindow<ShipWindow>([ship.Symbol]);
        }
    }

    private void OpenOrbital()
    {
        if (OrbitalsListBox is null)
            return;
        var listbox = OrbitalsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Waypoint.Orbitals.Count)
        {
            var orbital = Waypoint.Orbitals[index];
            RootScreen.ShowWindow<WaypointWindow>([orbital, Waypoint.SystemSymbol]);
        }
    }
}
