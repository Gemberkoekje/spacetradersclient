using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class NavigationWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Navigation? Navigation { get; set; }
    private ShipService ShipService { get; init; }

    public NavigationWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var navigation = data.First(s => s.Symbol == Symbol).Navigation;
        if (Navigation is not null && Navigation == navigation)
            return Task.CompletedTask;
        Title = $"Navigation for ship {Symbol}";
        Navigation = navigation;
        DrawContent();
        return Task.CompletedTask;
    }

    public void SetSymbol(string symbol, string? _)
    {
        Symbol = symbol;
        LoadData(ShipService.GetShips().ToArray());
    }

    private void DrawContent()
    {
        Clean();
        if (Navigation is null)
        {
            Controls.AddLabel($"Navigation data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"System:", 2, y);
        Controls.AddButton($"{Navigation.SystemSymbol}", 10, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow>(Navigation.SystemSymbol));
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
