using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
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

    private CustomListBox ModulesListBox { get; set; }

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
        if (Surface == null)
            return Task.CompletedTask;
        var modules = data.First(s => s.Symbol == Symbol).Modules;
        if (Modules is not null && Modules == modules)
            return Task.CompletedTask;
        Title = $"Modules for ship {Symbol}";
        Modules = modules;
        Binds["Modules"].SetData([.. Modules.Select(module => $"{module.Name} (Capacity: {module.Capacity}, Range: {module.Range})")]);
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
        ModulesListBox = Controls.AddListbox($"Modules", 2, y, 80, 10);
        Binds.Add("Modules", ModulesListBox);
        y += 10;
        Controls.AddButton("Show Module", 2, y++, (_, _) => OpenModule());
    }

    private void OpenModule()
    {
        var listbox = ModulesListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < Modules.Count)
        {
            var module = Modules[index];
            RootScreen.ShowWindow<ModuleWindow>([module.Symbol.ToString()]);
        }
    }
}
