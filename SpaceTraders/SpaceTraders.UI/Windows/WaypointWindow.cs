using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpaceTraders.UI.Windows;

internal sealed class WaypointWindow : ClosableWindow, ICanLoadData<Waypoint>, ICanLoadData<Ship[]>
{
    private Waypoint? Waypoint { get; set; }

    private WaypointService WaypointService { get; }

    private Ship[] Ships { get; set; } = [];

    public WaypointWindow(RootScreen rootScreen, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        WaypointService = waypointService;
    }

    public void LoadData(Waypoint data)
    {
        Title = $"Waypoint {data.Symbol}";
        Waypoint = data;
        DrawContent();
    }

    public void LoadData(Ship[] data)
    {
        Ships = data.Where(d => d.Navigation.WaypointSymbol == Waypoint?.Symbol).ToArray();
        DrawContent();
    }

    private void DrawContent()
    {
        foreach (var c in Controls.Where(c => c.Name != "CloseButton").ToList())
        {
            Controls.Remove(c);
        }
        if (Waypoint is null)
            return;

        var y = 2;
        Controls.AddLabel($"Symbol: {Waypoint.Symbol}", 2, y++);
        Controls.AddLabel($"Type: {Waypoint.Type}", 2, y++);
        Controls.AddLabel($"Location: {Waypoint.X}, {Waypoint.Y}", 2, y++);
        if (Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Shipyard))
        {
            Controls.AddLabel($"Shipyard (will become a button eventually)", 2, y++);
        }
        if (Waypoint.Traits.Any(t => t.Symbol == WaypointTraitSymbol.Marketplace))
        {
            Controls.AddLabel($"Marketplace (will become a button eventually)", 2, y++);
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
                Controls.AddAsyncButton($"{ship.Symbol} ({ship.Registration.Role})", 4, y++, async () => RootScreen.ShowWindow<ShipWindow, Ship>(ship), (e) => RootScreen.ShowWindow<WarningWindow, string>(string.Join(", ", e.Message)));
            }
        }
        y++;
        if (Waypoint.Orbitals.Count > 0)
        {
            Controls.AddLabel($"Orbitals:", 2, y++);
            foreach (var orbital in Waypoint.Orbitals.OrderBy(w => w))
            {
                Controls.AddAsyncButton($"{orbital})", 4, y++, async () => RootScreen.ShowWindow<WaypointWindow, Waypoint>(await WaypointService.GetWaypoint(Waypoint.SystemSymbol, orbital)), (e) => RootScreen.ShowWindow<WarningWindow, string>(string.Join(", ", e.Message)));
            }
        }
        if (!string.IsNullOrEmpty(Waypoint.Orbits))
        {
            Controls.AddLabel($"Orbits:", 2, y);
            Controls.AddAsyncButton($"{Waypoint.Orbits}", 11, y++, async () => RootScreen.ShowWindow<WaypointWindow, Waypoint>(await WaypointService.GetWaypoint(Waypoint.SystemSymbol, Waypoint.Orbits)), (e) => RootScreen.ShowWindow<WarningWindow, string>(string.Join(", ", e.Message)));
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
