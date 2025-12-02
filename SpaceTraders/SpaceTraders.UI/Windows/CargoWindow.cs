using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class CargoWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Cargo? Cargo { get; set; }
    private ShipService ShipService { get; init; }

    public CargoWindow(RootScreen rootScreen, ShipService shipService)
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
        var frame = data.First(s => s.Symbol == Symbol).Cargo;
        if (Cargo is not null && Cargo == frame)
            return Task.CompletedTask;
        Title = $"Cargo for ship {Symbol}";
        Cargo = frame;
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
        if (Cargo is null)
        {
            Controls.AddLabel($"Frame data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach (var item in Cargo.Inventory)
        {
            Controls.AddLabel($"Item: {item.Name}, Quantity: {item.Units}", 2, y++);
        }
        y++;
        Controls.AddLabel($"Total: {Cargo.Units} / {Cargo.Capacity}", 2, y++);
        ResizeAndRedraw();
    }
}
