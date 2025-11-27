using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipsWindow : ClosableWindow, ICanLoadData<Ship[]>
{
    private Ship[] Ships { get; set; } = [];

    public ShipsWindow(RootScreen rootScreen)
        : base(rootScreen, 52, 11)
    {
    }

    public void LoadData(Ship[] data)
    {
        Ships = data;
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
