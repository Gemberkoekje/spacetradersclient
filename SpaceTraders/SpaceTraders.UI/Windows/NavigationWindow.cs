using Qowaiv.Validation.Abstractions;
using SadConsole.UI.Controls;
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

internal sealed class NavigationWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private ImmutableList<WaypointDistance> Waypoints { get; set; } = [];

    private Navigation? Navigation { get; set; }
    private ShipService ShipService { get; init; }
    private ShipNavService ShipNavService { get; init; }
    private WaypointService WaypointService { get; init; }

    public NavigationWindow(RootScreen rootScreen, ShipService shipService, ShipNavService shipNavService, WaypointService waypointService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        ShipNavService = shipNavService;
        WaypointService = waypointService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var navigation = data.First(s => s.Symbol == Symbol).Navigation;
        if (Navigation is not null && Navigation == navigation)
            return Task.CompletedTask;
        Title = $"Navigation for ship {Symbol}";
        Navigation = navigation;
        var waypointsForSystem = WaypointService.GetWaypoints().GetValueOrDefault(Navigation.SystemSymbol);
        var orderedWaypoints = waypointsForSystem.OrderBy(wp => GetDistance((wp.X, wp.Y)));
        Waypoints = orderedWaypoints.Select(wp => new WaypointDistance(wp.Symbol, GetDistance((wp.X, wp.Y)))).ToImmutableList();
        DrawContent();
        return Task.CompletedTask;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
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
        Controls.AddButton($"{Navigation.SystemSymbol}", 10, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow>([Navigation.SystemSymbol]));
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
        y++;
        Controls.AddButton($"Dock", 2, y++, (_, _) => RootScreen.ScheduleCommand(Dock));
        Controls.AddButton($"Orbit", 2, y++, (_, _) => RootScreen.ScheduleCommand(Orbit));
        y++;
        ListBox list = new(40, 7)
        {
            Name = "WaypointList",
            DrawBorder = true,
        };
        foreach (var wp in Waypoints)
            list.Items.Add(wp);

        Controls.AddLabel($"Waypoints:", 2, y++);
        list.Position = (2, y);
        Controls.Add(list);
        y += 7;
        Controls.AddButton($"Navigate to waypoint", 2, y++, (_, _) => RootScreen.ScheduleCommand(Navigate));
        ResizeAndRedraw();
    }

    private async Task Dock()
    {
        var result = await ShipNavService.Dock(Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWarningWindow(Result.WithMessages(ValidationMessage.Info("Ship successfully docked")));
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private async Task Navigate()
    {
        var list = Controls.FirstOrDefault(c => c.Name == "WaypointList") as ListBox;
        var selectedWaypoint = list.SelectedItem as WaypointDistance;
        var result = await ShipNavService.Navigate(Symbol, selectedWaypoint.Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWarningWindow(Result.WithMessages(ValidationMessage.Info("Ship successfully navigating")));
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private async Task Orbit()
    {
        var result = await ShipNavService.Orbit(Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWarningWindow(Result.WithMessages(ValidationMessage.Info("Ship successfully orbited")));
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private double GetDistance((int X, int Y) destination)
    {
        var mysystem = WaypointService.GetWaypoints().GetValueOrDefault(Navigation.SystemSymbol);
        var mywaypoint = mysystem.FirstOrDefault(s => s.Symbol == Navigation.WaypointSymbol);
        var mylocation = (mywaypoint.X, mywaypoint.Y);
        var diffX = Math.Abs(destination.X - mywaypoint.X);
        var diffY = Math.Abs(destination.Y - mywaypoint.Y);
        return Math.Sqrt(diffX * diffX + diffY * diffY);
    }

    private record WaypointDistance(string Symbol, double Distance)
    {
        public override string ToString() => $"{Symbol} - Distance: {Distance:F2}";
    }
}
