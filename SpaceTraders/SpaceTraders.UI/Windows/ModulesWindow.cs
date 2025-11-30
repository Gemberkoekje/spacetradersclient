using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ModulesWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private ImmutableList<Module> Modules { get; set; }
    private ShipService ShipService { get; init; }

    public ModulesWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var modules = data.First(s => s.Symbol == Symbol).Modules;
        if (Modules is not null && Modules == modules)
            return Task.CompletedTask;
        Title = $"Modules for ship {Symbol}";
        Modules = modules;
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
        if (Modules is null)
        {
            Controls.AddLabel($"Modules loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        foreach(var module in Modules)
        {
            Controls.AddButton($"{module.Name} (Capacity: {module.Capacity}, Range: {module.Range})", 2, y++, (_, _) => RootScreen.ShowWindow<ModuleWindow>([module.Symbol.ToString()]));
        }

        ResizeAndRedraw();
    }
}
