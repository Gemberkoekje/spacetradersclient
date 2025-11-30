using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipsWindow : ClosableWindow
{
    private Ship[] Ships { get; set; } = [];

    public ShipsWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 11)
    {
        shipService.Updated += LoadData;
        LoadData(shipService.GetShips().ToArray());
    }

    public Task LoadData(Ship[] data)
    {
        if (Ships.All(s => s == data.FirstOrDefault(d => d.Symbol == s.Symbol)) && data.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;

        Ships = data;
        Title = $"Ships";
        DrawContent();
        return Task.CompletedTask;
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
            Controls.AddButton($"{ship.Registration.Name} ({ship.Registration.Role} {ship.Frame.Name} at {ship.Navigation.WaypointSymbol} in {ship.Navigation.SystemSymbol})", 2, y++, (_, _) => RootScreen.ShowWindow<ShipWindow>([ship.Symbol]));
        }
        ResizeAndRedraw();
    }

}
