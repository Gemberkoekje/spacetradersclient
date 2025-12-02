using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardMountsWindow : ClosableWindow, ICanSetSymbols
{
    private string ShipSymbol { get; set; } = string.Empty;
    private string SystemSymbol { get; set; } = string.Empty;
    private string WaypointSymbol { get; set; } = string.Empty;

    private ImmutableList<Mount> Mounts { get; set; }
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
        var shipyardShip = shipyard.Ships.FirstOrDefault(s => s.Type.ToString() == ShipSymbol);
        Mounts = shipyardShip?.Mounts ?? ImmutableList<Mount>.Empty;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Mounts is null)
        {
            Controls.AddLabel($"Mounts loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach (var mount in Mounts)
        {
            Controls.AddButton($"{mount.Name} (Strength: {mount.Strength}{(mount.Deposits.Any() ? $", Deposits: {mount.Deposits.Count}" : "")})", 2, y++, (_, _) => RootScreen.ShowWindow<MountWindow>([mount.Symbol.ToString()]));
        }

        ResizeAndRedraw();
    }
}
