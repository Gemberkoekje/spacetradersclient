using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class MountsWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private ImmutableList<Mount> Mounts { get; set; }
    private ShipService ShipService { get; init; }

    public MountsWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var mounts = data.First(s => s.Symbol == Symbol).Mounts;
        if (Mounts is not null && Mounts == mounts)
            return Task.CompletedTask;
        Title = $"Mounts for ship {Symbol}";
        Mounts = mounts;
        DrawContent();
        return Task.CompletedTask;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(ShipService.GetShips().ToArray());
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
