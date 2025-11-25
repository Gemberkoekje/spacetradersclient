using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

internal class ShipWindow : ClosableWindow
{
    private Ship Ship { get; init; }

    public ShipWindow(Ship ship, RootScreen rootScreen)
        : base(rootScreen, 52, 20)
    {
        Title = $"Ship {ship.Symbol}";
        Ship = ship;
        DrawContent();
    }

    private void DrawContent()
    {
        if (Ship is null)
            return;
        var y = 2;
        Controls.AddLabel($"Symbol: {Ship.Symbol}", 2, y++);
        Controls.AddLabel($"Name: {Ship.Registration.Name}", 2, y++);
        Controls.AddLabel($"Faction: {Ship.Registration.FactionSymbol}", 2, y++);
        Controls.AddLabel($"Role: {Ship.Registration.Role}", 2, y++);
        Controls.AddLabel($"Navigation:", 2, y);
        Controls.AddButton($"{Ship.Navigation.Status} {(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? "to" : "at")} {Ship.Navigation.Route.Destination.Symbol}{(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? $" until {Ship.Navigation.Route.ArrivalTime}" : "")}", 15, y++, (_, _) => RootScreen.ShowWindow<NavigationWindow, Navigation>(Ship.Navigation));
        ResizeAndRedraw();
    }
}
