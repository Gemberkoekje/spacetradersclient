using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
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
        var cargo = data.First(s => s.Symbol == Symbol).Cargo;
        if (Cargo is not null && Cargo == cargo)
            return Task.CompletedTask;
        Title = $"Cargo for ship {Symbol}";
        Cargo = cargo;

        Binds["CargoList"].SetData(Cargo.Inventory.Select(c => $"{new string(' ', 5 - c.Units.ToString("#.###").Length)}{c.Units:#.###} {c.Name}").ToArray());
        Binds["Total"].SetData([$"{Cargo.Units} / {Cargo.Capacity}"]);
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
        Binds.Add("CargoList", Controls.AddListbox($"CargoList", 20, y, 80, 10));
        y += 10;
        Controls.AddLabel($"Total:", "TotalLabel", 2, y);
        Binds.Add("Total", Controls.AddLabel($"Total", "Total", 20, y++));
    }
}
