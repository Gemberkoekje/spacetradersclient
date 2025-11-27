using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : ClosableWindow, ICanLoadData<SystemWaypoint>
{
    private SystemWaypoint? System { get; set; }

    private SystemService SystemService { get; init; }

    public SystemDataWindow(RootScreen rootScreen, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        SystemService = systemService;
    }

    public void LoadData(SystemWaypoint data)
    {
        Title = $"System {data.Symbol}";
        System = data;
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
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow, SystemWaypoint>(SystemService.GetSystem(System.Symbol)));

        ResizeAndRedraw();
    }
}
