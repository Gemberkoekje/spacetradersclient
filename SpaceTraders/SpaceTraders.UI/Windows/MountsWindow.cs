using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
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

    private CustomListBox MountsListBox { get; set; }

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
        if (Surface == null)
            return Task.CompletedTask;
        var mounts = data.First(s => s.Symbol == Symbol).Mounts;
        if (Mounts is not null && Mounts == mounts)
            return Task.CompletedTask;
        Title = $"Mounts for ship {Symbol}";
        Mounts = mounts;
        Binds["Mounts"].SetData([.. Mounts.Select(mount => $"{mount.Name} (Strength: {mount.Strength}{(mount.Deposits.Any() ? $", Deposits: {mount.Deposits.Count}" : "")})")]);
        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(ShipService.GetShips().ToArray());
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
