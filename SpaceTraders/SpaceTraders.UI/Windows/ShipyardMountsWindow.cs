using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardMountsWindow : ClosableWindow, ICanSetSymbols
{
    private string ShipSymbol { get; set; } = string.Empty;

    private string SystemSymbol { get; set; } = string.Empty;

    private string WaypointSymbol { get; set; } = string.Empty;

    private ImmutableList<Mount> Mounts { get; set; }

    private CustomListBox MountsListBox { get; set; }

    private ShipyardService ShipyardService { get; init; }

    public ShipyardMountsWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        ShipSymbol = symbols[0];
        WaypointSymbol = symbols[1];
        SystemSymbol = symbols[2] ?? string.Empty;
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        if (Surface == null)
            return;
        var shipyard = data.GetValueOrDefault(SystemSymbol).First(s => s.Symbol == WaypointSymbol);

        Title = $"Shipyard {shipyard.Symbol}";
        var ship = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        Mounts = ship?.Mounts ?? [];

        Binds["Mounts"].SetData([.. Mounts.Select(mount => $"{mount.Name} (Strength: {mount.Strength}{(mount.Deposits.Any() ? $", Deposits: {mount.Deposits.Count}" : "")})")]);

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        MountsListBox = Controls.AddListbox($"Mounts", 2, y, 80, 10);
        Binds.Add("Mounts", MountsListBox);
        y += 10;
        Controls.AddButton("Show Mount", 2, y++, (_, _) => OpenMount());
    }

    private void OpenMount()
    {
        var listbox = MountsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Mounts.Count)
        {
            var mount = Mounts[index];
            RootScreen.ShowWindow<MountWindow>([mount.Symbol.ToString()]);
        }
    }
}
