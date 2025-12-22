using Qowaiv.Validation.Abstractions;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class RouteWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private ImmutableList<WaypointDistance> Waypoints { get; set; } = [];

    private CustomListBox WaypointsListBox { get; set; }

    private Navigation? Navigation { get; set; }
    private ShipService ShipService { get; init; }
    private ShipNavService ShipNavService { get; init; }
    private WaypointService WaypointService { get; init; }

    public RouteWindow(RootScreen rootScreen, ShipService shipService, ShipNavService shipNavService, WaypointService waypointService)
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
        var navigation = data.FirstOrDefault(s => s.Symbol == Symbol)?.Navigation;
        if (navigation == null)
            return Task.CompletedTask;
        if (Navigation is not null && Navigation == navigation)
            return Task.CompletedTask;
        Title = $"Navigation for ship {Symbol}";
        Navigation = navigation;
        var waypointsForSystem = WaypointService.GetWaypoints().GetValueOrDefault(Navigation.SystemSymbol);
        var orderedWaypoints = waypointsForSystem.OrderBy(wp => GetDistance((wp.X, wp.Y)));
        Waypoints = orderedWaypoints.Select(wp => new WaypointDistance(wp.Symbol, GetDistance((wp.X, wp.Y)))).ToImmutableList();

        Binds["Destination.Symbol"].SetData([$"{Navigation.Route.Destination.Symbol}"]);
        Binds["Destination.SystemSymbol"].SetData([$"{Navigation.Route.Destination.SystemSymbol}"]);
        Binds["Destination.Type"].SetData([$"{Navigation.Route.Destination.Type}"]);

        Binds["Origin.Symbol"].SetData([$"{Navigation.Route.Origin.Symbol}"]);
        Binds["Origin.SystemSymbol"].SetData([$"{Navigation.Route.Origin.SystemSymbol}"]);
        Binds["Origin.Type"].SetData([$"{Navigation.Route.Origin.Type}"]);

        Binds["DepartureTime"].SetData([$"{Navigation.Route.DepartureTime:yyyy-MM-dd HH:mm:ss}"]);
        Binds["ArrivalTime"].SetData([$"{Navigation.Route.ArrivalTime:yyyy-MM-dd HH:mm:ss}"]);

        Binds["WaypointList"].SetData([.. Waypoints.Select(w => w)]);

        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(ShipService.GetShips().ToArray());
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Destination:", 2, y++);
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Destination.Symbol", Controls.AddLabel($"Navigation.Route.Destination.Symbol", 20, y++));
        Controls.AddLabel($"System:", 2, y);
        Binds.Add("Destination.SystemSymbol", Controls.AddLabel($"Navigation.Route.Destination.SystemSymbol", 20, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Destination.Type", Controls.AddLabel($"Navigation.Route.Destination.Type", 20, y++));
        y++;
        Controls.AddLabel($"Origin:", 2, y++);
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Origin.Symbol", Controls.AddLabel($"Navigation.Route.Origin.Symbol", 20, y++));
        Controls.AddLabel($"System:", 2, y);
        Binds.Add("Origin.SystemSymbol", Controls.AddLabel($"Navigation.Route.Origin.SystemSymbol", 20, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Origin.Type", Controls.AddLabel($"Navigation.Route.Origin.Type", 20, y++));
        Controls.AddLabel($"Departure time:", 2, y);
        Binds.Add("DepartureTime", Controls.AddLabel($"Navigation.Route.DepartureTime", 20, y++));
        Controls.AddLabel($"Arrival time:", 2, y);
        Binds.Add("ArrivalTime", Controls.AddLabel($"Navigation.Route.ArrivalTime", 20, y++));
        y++;
        Controls.AddButton($"Dock", 2, y++, (_, _) => RootScreen.ScheduleCommand(Dock));
        Controls.AddButton($"Orbit", 2, y++, (_, _) => RootScreen.ScheduleCommand(Orbit));
        y++;
        WaypointsListBox = Controls.AddListbox("WaypointList", 2, y, 40, 7);
        Binds.Add("WaypointList", WaypointsListBox);
        y+= 7;
        Controls.AddButton($"Navigate to waypoint", 2, y++, (_, _) => RootScreen.ScheduleCommand(Navigate));
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
        var list = WaypointsListBox;
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
