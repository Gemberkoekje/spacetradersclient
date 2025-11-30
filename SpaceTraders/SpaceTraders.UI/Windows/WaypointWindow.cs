using SpaceTraders.Core.Enums;
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

internal sealed class WaypointWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private string ParentSymbol { get; set; } = string.Empty;

    private Waypoint? Waypoint { get; set; }

    private Ship[] Ships { get; set; } = [];

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
        var waypoints = data.GetValueOrDefault(ParentSymbol);
        var waypoint = waypoints?.FirstOrDefault(d => d.Symbol == Symbol);
        if (Waypoint is not null && Waypoint == waypoint)
            return Task.CompletedTask;

        Title = $"Waypoint {Symbol} in {ParentSymbol}";
        Waypoint = waypoint;
        DrawContent();
        return Task.CompletedTask;
    }

    public Task LoadData(Ship[] data)
    {
        var relevantData = data.Where(d => d.Navigation.WaypointSymbol == Waypoint?.Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;
        Ships = relevantData;
        DrawContent();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        Clean();
        if (Waypoint is null)
        {
            Controls.AddLabel($"Waypoint not found.", 2, 2);
            ResizeAndRedraw();
            return;
        }

        var y = 2;
        Controls.AddLabel($"Symbol: {Waypoint.Symbol}", 2, y++);
        Controls.AddLabel($"Type: {Waypoint.Type}", 2, y++);
        Controls.AddLabel($"Location: {Waypoint.X}, {Waypoint.Y}", 2, y++);
        if (Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard))
        {
            Controls.AddButton($"Shipyard", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardWindow>([Symbol, ParentSymbol]));
        }
        if (Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace))
        {
            Controls.AddButton($"Marketplace", 2, y++, (_, _) => RootScreen.ShowWindow<MarketWindow>([Symbol, ParentSymbol]));
        }
        if (Waypoint.IsUnderConstruction)
        {
            Controls.AddLabel($"Is Under Construction (will become a button eventually)", 2, y++);
        }
        y++;
        if (Ships.Length > 0)
        {
            Controls.AddLabel($"Ships at this Waypoint:", 2, y++);
            foreach (var ship in Ships.OrderBy(s => s.Symbol))
            {
                Controls.AddButton($"{ship.Symbol} ({ship.Registration.Role})", 4, y++, (_, _) => RootScreen.ShowWindow<ShipWindow>([ship.Symbol]));
            }
        }
        y++;
        if (Waypoint.Orbitals.Count > 0)
        {
            Controls.AddLabel($"Orbitals:", 2, y++);
            foreach (var orbital in Waypoint.Orbitals.OrderBy(w => w))
            {
                Controls.AddButton($"{orbital}", 4, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([orbital, Waypoint.SystemSymbol]));
            }
        }
        if (!string.IsNullOrEmpty(Waypoint.Orbits))
        {
            Controls.AddLabel($"Orbits:", 2, y);
            Controls.AddButton($"{Waypoint.Orbits}", 11, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([Waypoint.Orbits, Waypoint.SystemSymbol]));
        }
        y++;
        Controls.AddLabel($"Traits:", 2, y++);
        foreach (var traits in Waypoint.Traits.Where(t => !IsSpecialTrait(t.Symbol)).OrderBy(w => w.Name))
        {
            Controls.AddLabel($"{traits.Name}", 4, y++);
        }
        if (Waypoint.Modifiers.Any())
        {
            Controls.AddLabel($"Modifiers:", 2, y++);
            foreach (var modifier in Waypoint.Modifiers)
            {
                Controls.AddLabel($"{modifier.Name}", 4, y++);
            }
        }
        ResizeAndRedraw();
    }

    private bool IsSpecialTrait(WaypointTraitSymbol trait)
    {
        return trait is WaypointTraitSymbol.Marketplace or
               WaypointTraitSymbol.Shipyard;
    }
}
