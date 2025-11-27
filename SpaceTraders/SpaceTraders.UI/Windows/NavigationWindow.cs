using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

internal sealed class NavigationWindow : ClosableWindow, ICanLoadData<Navigation>
{
    private Navigation? Navigation { get; set; }

    private SystemService SystemService { get; init; }

    public NavigationWindow(RootScreen rootScreen, SystemService systemService)
        : base(rootScreen, 52, 20)
    {
        SystemService = systemService;
    }

    public void LoadData(Navigation data)
    {
        Title = $"Navigation for ship {data.ShipSymbol}";
        Navigation = data;
        DrawContent();
    }

    private void DrawContent()
    {
        if (Navigation is null)
            return;
        var y = 2;
        Controls.AddLabel($"System:", 2, y);
        Controls.AddButton($"{Navigation.SystemSymbol}", 10, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow, SystemWaypoint>(SystemService.GetSystem(Navigation.SystemSymbol)));
        Controls.AddLabel($"Waypoint: {Navigation.WaypointSymbol}", 2, y++);
        Controls.AddLabel($"Destination: {Navigation.Route.Destination.Symbol}", 2, y++);
        Controls.AddLabel($"Destination: {Navigation.Route.Destination.SystemSymbol}", 2, y++);
        Controls.AddLabel($"Destination: {Navigation.Route.Destination.Type}", 2, y++);
        Controls.AddLabel($"Origin: {Navigation.Route.Origin.Symbol}", 2, y++);
        Controls.AddLabel($"Origin: {Navigation.Route.Origin.SystemSymbol}", 2, y++);
        Controls.AddLabel($"Origin: {Navigation.Route.Origin.Type}", 2, y++);
        Controls.AddLabel($"DepartureTime: {Navigation.Route.DepartureTime}", 2, y++);
        Controls.AddLabel($"ArrivalTime: {Navigation.Route.ArrivalTime}", 2, y++);
        Controls.AddLabel($"Status: {Navigation.Status}", 2, y++);
        Controls.AddLabel($"FlightMode: {Navigation.FlightMode}", 2, y++);
        ResizeAndRedraw();
    }
}
