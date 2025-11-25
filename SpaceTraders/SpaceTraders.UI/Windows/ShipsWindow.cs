using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

internal class ShipsWindow : ClosableWindow
{
    Ship[] Ships { get; init; }

    public ShipsWindow(Ship[] ships, RootScreen rootScreen)
        : base(rootScreen, 52, 11)
    {
        Ships = ships;
        Title = $"Ships";
        DrawContent();
    }

    private void DrawContent()
    {
        int y = 2;
        foreach (var ship in Ships)
        {
            Controls.AddButton($"{ship.Symbol}", 2, y++, (_, _) => RootScreen.ShowWindow<ShipWindow, Ship>(ship));
        }
        ResizeAndRedraw();
    }
}
