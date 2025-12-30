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
        if (Surface == null)
            return Task.CompletedTask;
        var navigation = data.First(s => s.Symbol == Symbol).Navigation;
        if (Navigation is not null && Navigation == navigation)
            return Task.CompletedTask;
        Title = $"Navigation for ship {Symbol}";
        Navigation = navigation;

        Binds["SystemSymbol"].SetData([$"{Navigation.SystemSymbol}"]);
        Binds["WaypointSymbol"].SetData([$"{Navigation.WaypointSymbol}"]);
        Binds["Status"].SetData([$"{BuildStatus(navigation)}"]);
        Binds["FlightMode"].SetData([$"{Navigation.FlightMode}"]);

        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(ShipService.GetShips().ToArray());
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

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"System:", 2, y);
        Binds.Add("SystemSymbol", Controls.AddButton($"Navigation.SystemSymbol", 15, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow>([Navigation.SystemSymbol])));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("WaypointSymbol", Controls.AddLabel($"Navigation.WaypointSymbol", 15, y++));
        Controls.AddLabel($"Status:", 2, y);
        Binds.Add("Status", Controls.AddLabel($"Navigation.Status", 15, y++));
        Controls.AddLabel($"Flight Mode:", 2, y);
        Binds.Add("FlightMode", Controls.AddLabel($"Navigation.FlightMode", 15, y++));
        Controls.AddButton($"Route", 15, y++, (_, _) => RootScreen.ShowWindow<RouteWindow>([Symbol]));
    }
}
