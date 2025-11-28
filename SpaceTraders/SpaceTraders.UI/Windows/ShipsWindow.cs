using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipsWindow : ClosableWindow, ICanLoadData<Ship[]>
{
    private Ship[] Ships { get; set; } = [];

    public ShipsWindow(RootScreen rootScreen)
        : base(rootScreen, 52, 11)
    {
        DrawContent();
    }

    public void LoadData(Ship[] data)
    {
        if (Ships.All(s => s == data.FirstOrDefault(d => d.Symbol == s.Symbol)) && data.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return;

        Ships = data;
        Title = $"Ships";
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Ships.Length == 0)
        {
            Controls.AddLabel($"Ships loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        int y = 2;
        foreach (var ship in Ships)
        {
            Controls.AddButton($"{ship.Symbol}", 2, y++, (_, _) => RootScreen.ShowWindow<ShipWindow>(ship.Symbol));
        }
        ResizeAndRedraw();
    }

}
