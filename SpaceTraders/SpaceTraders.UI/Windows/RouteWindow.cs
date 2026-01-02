using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class RouteWindow : DataBoundWindowWithContext<Navigation, ShipContext>
{
    private readonly ShipService _shipService;
    private readonly ShipNavService _shipNavService;
    private readonly WaypointService _waypointService;

    private ImmutableArray<WaypointDistance> _waypoints = [];
    private CustomListBox? _waypointsListBox;

    public RouteWindow(RootScreen rootScreen, ShipService shipService, ShipNavService shipNavService, WaypointService waypointService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;
        _shipNavService = shipNavService;
        _waypointService = waypointService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override Navigation? FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship)?.Navigation;

    protected override void BindData(Navigation data)
    {
        Title = $"Navigation for ship {Context.Ship}";

        var waypointsForSystem = _waypointService.GetWaypoints().GetValueOrDefault(data.SystemSymbol);
        if (!waypointsForSystem.IsDefault)
        {
            var orderedWaypoints = waypointsForSystem.OrderBy(wp => GetDistance(data, (wp.X, wp.Y)));
            _waypoints = [.. orderedWaypoints.Select(wp => new WaypointDistance(wp.Symbol, GetDistance(data, (wp.X, wp.Y))))];
        }

        Binds["Destination.Symbol"].SetData([$"{data.Route.Destination.Symbol}"]);
        Binds["Destination.SystemSymbol"].SetData([$"{data.Route.Destination.SystemSymbol}"]);
        Binds["Destination.Type"].SetData([$"{data.Route.Destination.Type}"]);

        Binds["Origin.Symbol"].SetData([$"{data.Route.Origin.Symbol}"]);
        Binds["Origin.SystemSymbol"].SetData([$"{data.Route.Origin.SystemSymbol}"]);
        Binds["Origin.Type"].SetData([$"{data.Route.Origin.Type}"]);

        Binds["DepartureTime"].SetData([$"{data.Route.DepartureTime:yyyy-MM-dd HH:mm:ss}"]);
        Binds["ArrivalTime"].SetData([$"{data.Route.ArrivalTime:yyyy-MM-dd HH:mm:ss}"]);

        Binds["WaypointList"].SetData([.. _waypoints.Select(w => w)]);
    }

    protected override void DrawContent()
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
        _waypointsListBox = Controls.AddListbox("WaypointList", 2, y, 40, 7);
        Binds.Add("WaypointList", _waypointsListBox);
        y += 7;
        Controls.AddButton($"Navigate to waypoint", 2, y, (_, _) => RootScreen.ScheduleCommand(Navigate));
    }

    private async Task Dock()
    {
        var result = await _shipNavService.Dock(Context.Ship);
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
        if (_waypointsListBox?.SelectedItem is not WaypointDistance selectedWaypoint)
            return;
        var result = await _shipNavService.Navigate(Context.Ship, selectedWaypoint.Symbol);
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
        var result = await _shipNavService.Orbit(Context.Ship);
        if (result.IsValid)
        {
            RootScreen.ShowWarningWindow(Result.WithMessages(ValidationMessage.Info("Ship successfully orbited")));
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private double GetDistance(Navigation navigation, (int X, int Y) destination)
    {
        var mysystem = _waypointService.GetWaypoints().GetValueOrDefault(navigation.SystemSymbol);
        if (mysystem.IsDefault) return 0;
        var mywaypoint = mysystem.FirstOrDefault(s => s.Symbol == navigation.WaypointSymbol);
        if (mywaypoint is null) return 0;
        var diffX = Math.Abs(destination.X - mywaypoint.X);
        var diffY = Math.Abs(destination.Y - mywaypoint.Y);
        return Math.Sqrt(diffX * diffX + diffY * diffY);
    }

    private sealed record WaypointDistance(WaypointSymbol Symbol, double Distance)
    {
        public override string ToString() => $"{Symbol} - Distance: {Distance:F2}";
    }
}
