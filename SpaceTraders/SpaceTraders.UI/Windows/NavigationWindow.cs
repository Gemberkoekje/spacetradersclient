using Qowaiv.Validation.Abstractions;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class NavigationWindow : DataBoundWindowWithContext<Navigation, ShipContext>
{
    private readonly ShipService _shipService;

    public NavigationWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;

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
        Binds["SystemSymbol"].SetData([$"{data.SystemSymbol}"]);
        Binds["WaypointSymbol"].SetData([$"{data.WaypointSymbol}"]);
        Binds["Status"].SetData([$"{BuildStatus(data)}"]);
        Binds["FlightMode"].SetData([$"{data.FlightMode}"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"System:", 2, y);
        Binds.Add("SystemSymbol", Controls.AddButton($"Navigation.SystemSymbol", 15, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow, SystemContext>(new (CurrentData!.SystemSymbol))));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("WaypointSymbol", Controls.AddLabel($"Navigation.WaypointSymbol", 15, y++));
        Controls.AddLabel($"Status:", 2, y);
        Binds.Add("Status", Controls.AddLabel($"Navigation.Status", 15, y++));
        Controls.AddLabel($"Flight Mode:", 2, y);
        Binds.Add("FlightMode", Controls.AddLabel($"Navigation.FlightMode", 15, y++));
        Controls.AddButton($"Route", 15, y, (_, _) => RootScreen.ShowWindow<RouteWindow, ShipContext>(Context));
    }

    private static string BuildStatus(Navigation navigation)
    {
        if (navigation.Status == ShipNavStatus.Docked)
            return $"Docked at {navigation.WaypointSymbol}";
        if (navigation.Status == ShipNavStatus.InOrbit)
            return $"In orbit of {navigation.WaypointSymbol}";
        if (navigation.Status == ShipNavStatus.InTransit)
        {
            return $"In transit from {navigation.Route.Origin.Symbol} to {navigation.Route.Destination.Symbol}, arriving at {navigation.Route.ArrivalTime:yyyy-MM-dd HH:mm}";
        }
        return $"{navigation.Status}";
    }
}
