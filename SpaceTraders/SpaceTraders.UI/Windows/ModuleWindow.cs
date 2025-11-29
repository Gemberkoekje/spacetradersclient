using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ModuleWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private Module? Module { get; set; }
    private ShipService ShipService { get; init; }

    public ModuleWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var module = data.First(s => s.Symbol == ParentSymbol).Modules.First(m => m.Symbol.ToString() == Symbol);
        if (Module is not null && Module == module)
            return Task.CompletedTask;
        Title = $"Module for ship {Symbol}";
        Module = module;
        DrawContent();
        return Task.CompletedTask;
    }

    public void SetSymbol(string symbol, string? parentSymbol)
    {
        Symbol = symbol;
        ParentSymbol = parentSymbol ?? string.Empty;
        LoadData(ShipService.GetShips().ToArray());
    }

    private void DrawContent()
    {
        Clean();
        if (Module is null)
        {
            Controls.AddLabel($"Module data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Module.Name}", 2, y++);
        Controls.AddLabel($"Capacity: {Module.Capacity}", 2, y++);
        Controls.AddLabel($"Range: {Module.Range}", 2, y++);
        Controls.AddLabel($"Power Requirements: {Module.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Module.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Module.Requirements.Slots}", 2, y++);
        ResizeAndRedraw();
    }
}
