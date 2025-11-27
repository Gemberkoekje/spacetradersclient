using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : ClosableWindow, ICanLoadData<SystemWaypoint>, ICanLoadData<Ship[]>
{
    private SystemWaypoint? System { get; set; }

    private Ship[] Ships { get; set; } = [];

    private SystemService SystemService { get; init; }

    private WaypointService WaypointService { get; init; }

    public SystemDataWindow(RootScreen rootScreen, SystemService systemService, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        SystemService = systemService;
        WaypointService = waypointService;
    }

    public void LoadData(SystemWaypoint data)
    {
        Title = $"System {data.Symbol}";
        System = data;
        DrawContent();
    }

    public void LoadData(Ship[] data)
    {
        Ships = data.Where(d => d.Navigation.SystemSymbol == System.Symbol).ToArray();
        DrawContent();
    }

    private void DrawContent()
    {
        foreach (var c in Controls.Where(c => c.Name != "CloseButton").ToList())
        {
            Controls.Remove(c);
        }
        if (System is null)
            return;

        var y = 2;
        Controls.AddLabel($"Symbol: {System.Symbol}", 2, y++);
        Controls.AddLabel($"Name: {System.Name}", 2, y++);
        Controls.AddLabel($"Constellation: {System.Constellation}", 2, y++);
        Controls.AddLabel($"Sector: {System.SectorSymbol}", 2, y++);
        Controls.AddLabel($"Type: {System.SystemType}", 2, y++);
        Controls.AddLabel($"Position: {System.X}, {System.Y}", 2, y++);
        Controls.AddLabel($"Waypoints: {System.Waypoints.Count()}", 2, y++);
        Controls.AddLabel($"Factions: {System.Factions.Count()}", 2, y++);
        Controls.AddAsyncButton($"Show map", 2, y++, async () => RootScreen.ShowWindow<SystemMapWindow, SystemWaypoint>(await SystemService.GetSystemAsync(System.Symbol)), (e) => RootScreen.ShowWindow<WarningWindow, string>(string.Join(", ", e.Message)));
        y++;
        Controls.AddLabel($"Markets & Shipyards:", 2, y++);
        foreach (var wp in System.Waypoints.Where(w => w.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) || Ships.Any(s => s.Navigation.WaypointSymbol == w.Symbol)).OrderBy(w => w.Symbol))
        {
            var orbits = string.IsNullOrEmpty(wp.Orbits) ? null : System.Waypoints.FirstOrDefault(w => w.Symbol == wp.Orbits);
            if (orbits is not null && !orbits.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace || t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard))
            {
                Controls.AddLabel($"{orbits.Symbol} ({orbits.Type})", 2, y++, Color.Gray);
            }
            Controls.AddAsyncButton($"{wp.Symbol} ({(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace) ? "Marketplace" : string.Empty)}{(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) ? " & Shipyard" : string.Empty)})", string.IsNullOrEmpty(wp.Orbits) ? 2 : 4, y++, async () => RootScreen.ShowWindow<WaypointWindow, Waypoint>(await WaypointService.GetWaypoint(System.Symbol, wp.Symbol)), (e) => RootScreen.ShowWindow<WarningWindow, string>(string.Join(", ", e.Message)));
            foreach (var ship in Ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol).OrderBy(s => s.Symbol))
            {
                Controls.AddButton($"{ship.Symbol} ({ship.Registration.Role})", string.IsNullOrEmpty(wp.Orbits) ? 4 : 6, y++, (_, _) => RootScreen.ShowWindow<ShipWindow, Ship>(ship), Color.Cyan);
            }
        }

        ResizeAndRedraw();
    }
}
