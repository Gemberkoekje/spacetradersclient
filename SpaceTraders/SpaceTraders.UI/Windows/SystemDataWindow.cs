using Microsoft.Xna.Framework.Media;
using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private SystemWaypoint? System { get; set; }
    private Waypoint[] Waypoints { get; set; } = [];

    private Ship[] Ships { get; set; } = [];

    private readonly ShipService ShipService;
    private readonly SystemService SystemService;
    private readonly WaypointService WaypointService;

    public SystemDataWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        shipService.Updated += LoadData;
        systemService.Updated += LoadData;
        waypointService.Updated += LoadData;
        ShipService = shipService;
        SystemService = systemService;
        WaypointService = waypointService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(SystemService.GetSystems().ToArray());
        LoadData(WaypointService.GetWaypoints());
        LoadData(ShipService.GetShips().ToArray());
    }

    public void LoadData(SystemWaypoint[] data)
    {
        if (Surface == null)
            return;
        var system = data.FirstOrDefault(d => d.Symbol == Symbol);
        if (System is not null && System == system)
            return;
        Title = $"System {system.Symbol}";
        System = system;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var relevantData = data.Where(d => d.Navigation.SystemSymbol == System.Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;
        Ships = relevantData;
        DrawContent();
        return Task.CompletedTask;
    }

    public Task LoadData(ImmutableDictionary<string, ImmutableList<Waypoint>> data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var waypoints = data.GetValueOrDefault(Symbol);
        Waypoints = waypoints.ToArray();
        DrawContent();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        Clean();
        if (System is null)
        {
            Controls.AddLabel($"System loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }

        var y = 2;
        Controls.AddLabel($"Symbol: {System.Symbol}", 2, y++);
        Controls.AddLabel($"Name: {System.Name}", 2, y++);
        Controls.AddLabel($"Constellation: {System.Constellation}", 2, y++);
        Controls.AddLabel($"Sector: {System.SectorSymbol}", 2, y++);
        Controls.AddLabel($"Type: {System.SystemType}", 2, y++);
        Controls.AddLabel($"Position: {System.X}, {System.Y}", 2, y++);
        Controls.AddLabel($"Waypoints: {Waypoints.Count()}", 2, y++);
        Controls.AddLabel($"Factions: {System.Factions.Count()}", 2, y++);
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow>([System.Symbol]));
        y++;
        Controls.AddLabel($"Markets & Shipyards:", 2, y++);
        foreach (var wp in Waypoints.Where(w => w.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) || Ships.Any(s => s.Navigation.WaypointSymbol == w.Symbol)).OrderBy(w => w.Symbol))
        {
            var orbits = string.IsNullOrEmpty(wp.Orbits) ? null : Waypoints.FirstOrDefault(w => w.Symbol == wp.Orbits);
            if (orbits is not null && !orbits.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard))
            {
                Controls.AddLabel($"{orbits.Symbol} ({orbits.Type})", 2, y++, Color.Gray);
            }
            Controls.AddButton($"{wp.Symbol} ({(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace) ? "Marketplace" : string.Empty)}{(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) ? " & Shipyard" : string.Empty)})", string.IsNullOrEmpty(wp.Orbits) ? 2 : 4, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([wp.Symbol, wp.SystemSymbol]));
            foreach (var ship in Ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol).OrderBy(s => s.Symbol))
            {
                Controls.AddButton($"{ship.Symbol} ({ship.Registration.Role})", string.IsNullOrEmpty(wp.Orbits) ? 4 : 6, y++, (_, _) => RootScreen.ShowWindow<ShipWindow>([ship.Symbol]), Color.Cyan);
            }
        }

        ResizeAndRedraw();
    }
}
