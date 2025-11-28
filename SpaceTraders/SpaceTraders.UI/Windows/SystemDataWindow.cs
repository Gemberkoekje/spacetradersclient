using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : ClosableWindow, ICanLoadData<SystemWaypoint>, ICanLoadData<Ship[]>
{
    string? ICanLoadData.Symbol { get; set; }

    private SystemWaypoint? System { get; set; }

    private Ship[] Ships { get; set; } = [];

    public SystemDataWindow(RootScreen rootScreen)
        : base(rootScreen, 45, 30)
    {
        DrawContent();
    }

    Type ICanLoadData.DataType {
        get {
            if (System is null)
                return typeof(SystemWaypoint);
            return typeof(Ship[]);
        }
    }

    void ICanLoadData.LoadData(object data)
    {
        if (data is SystemWaypoint waypoint)
            LoadData(waypoint);
        else if (data is Ship[] ships)
            LoadData(ships);
        else
            throw new ArgumentException("Invalid data type for WaypointWindow", nameof(data));
    }

    public void LoadData(SystemWaypoint data)
    {
        if (System is not null && System == data)
            return;
        Title = $"System {data.Symbol}";
        System = data;
        DrawContent();
    }

    public void LoadData(Ship[] data)
    {
        var relevantData = data.Where(d => d.Navigation.SystemSymbol == System.Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return;
        Ships = relevantData;
        DrawContent();
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
        Controls.AddLabel($"Waypoints: {System.Waypoints.Count()}", 2, y++);
        Controls.AddLabel($"Factions: {System.Factions.Count()}", 2, y++);
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow>(System.Symbol));
        y++;
        Controls.AddLabel($"Markets & Shipyards:", 2, y++);
        foreach (var wp in System.Waypoints.Where(w => w.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) || Ships.Any(s => s.Navigation.WaypointSymbol == w.Symbol)).OrderBy(w => w.Symbol))
        {
            var orbits = string.IsNullOrEmpty(wp.Orbits) ? null : System.Waypoints.FirstOrDefault(w => w.Symbol == wp.Orbits);
            if (orbits is not null && !orbits.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard))
            {
                Controls.AddLabel($"{orbits.Symbol} ({orbits.Type})", 2, y++, Color.Gray);
            }
            Controls.AddButton($"{wp.Symbol} ({(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace) ? "Marketplace" : string.Empty)}{(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) ? " & Shipyard" : string.Empty)})", string.IsNullOrEmpty(wp.Orbits) ? 2 : 4, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>(wp.Symbol, wp.SystemSymbol));
            foreach (var ship in Ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol).OrderBy(s => s.Symbol))
            {
                Controls.AddButton($"{ship.Symbol} ({ship.Registration.Role})", string.IsNullOrEmpty(wp.Orbits) ? 4 : 6, y++, (_, _) => RootScreen.ShowWindow<ShipWindow>(ship.Symbol), Color.Cyan);
            }
        }

        ResizeAndRedraw();
    }
}
