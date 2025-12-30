using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipsWindow : ClosableWindow
{
    private Ship[] Ships { get; set; } = [];

    private CustomListBox? ShipsListBox { get; set; }

    public ShipsWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 11)
    {
        shipService.Updated += LoadData;
        DrawContent();
        LoadData(shipService.GetShips().ToArray());
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        if (Ships.All(s => s == data.FirstOrDefault(d => d.Symbol == s.Symbol)) && data.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;

        Ships = data;
        Title = $"Ships";
        Binds["Ships"].SetData([.. Ships.Select(ship => $"{ship.Registration.Name} ({ship.Registration.Role} {ship.Frame.Name} at {ship.Navigation.WaypointSymbol} in {ship.Navigation.SystemSymbol})")]);
        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        var y = 2;
        ShipsListBox = Controls.AddListbox($"Ships", 2, y, 80, 10);
        Binds.Add("Ships", ShipsListBox);
        y += 10;
        Controls.AddButton("Show Ship", 2, y++, (_, _) => OpenShip());
    }

    private void OpenShip()
    {
        if (ShipsListBox is null)
            return;
        var listbox = ShipsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Ships.Length)
        {
            var ship = Ships[index];
            RootScreen.ShowWindow<ShipWindow>([ship.Symbol]);
        }
    }
}
