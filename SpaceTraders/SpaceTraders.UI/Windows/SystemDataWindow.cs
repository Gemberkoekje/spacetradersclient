using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

internal class SystemDataWindow : ClosableWindow
{
    private SystemWaypoint System { get; init; }

    public SystemDataWindow(SystemWaypoint waypoint, RootScreen rootScreen)
        : base(rootScreen, 45, 30)
    {
        Title = $"System {waypoint.Symbol}";
        System = waypoint;
        DrawContent();
    }

    private void DrawContent()
    {
        if (System is null)
            return;

        var y = 2;
        Controls.AddLabel($"Symbol: {System.Symbol}", 2, y++);
        Controls.AddLabel($"Name: {System.Name}", 2, y++);
        Controls.AddLabel($"Constellation: {System.Constellation}", 2, y++);
        Controls.AddLabel($"Sector: {System.SectorSymbol}", 2, y++);
        Controls.AddLabel($"Type: {System.SystemType}", 2, y++);
        Controls.AddLabel($"Position: {System.X}, {System.Y}", 2, y++);
        Controls.AddLabel($"Waypoints: {System.Waypoints.Count}", 2, y++);
        Controls.AddLabel($"Factions: {System.Factions.Count}", 2, y++);
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow, SystemWaypoint>(RootScreen.GameSession.GetSystem(System.Symbol)));


        ResizeAndRedraw();
    }
}
