using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class CrewWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Crew? Crew { get; set; }
    private ShipService ShipService { get; init; }

    public CrewWindow(RootScreen rootScreen, ShipService shipService)
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
        var crew = data.First(s => s.Symbol == Symbol).Crew;
        if (Crew is not null && Crew == crew)
            return Task.CompletedTask;
        Title = $"Crew for ship {Symbol}";
        Crew = crew;

        Binds["Current"].SetData([$"{Crew.Current}"]);
        Binds["Capacity"].SetData([$"{Crew.Capacity}"]);
        Binds["Rotation"].SetData([$"{Crew.Rotation}"]);
        Binds["Morale"].SetData([$"{Crew.Morale}"]);
        Binds["Wages"].SetData([$"{Crew.Wages}"]);
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
        Controls.AddLabel($"Current Crew:", 2, y);
        Binds.Add("Current", Controls.AddLabel($"Crew.Current", 20, y++));
        Controls.AddLabel($"Crew Capacity:", 2, y);
        Binds.Add("Capacity", Controls.AddLabel($"Crew.Capacity", 20, y++));
        Controls.AddLabel($"Rotation:", 2, y);
        Binds.Add("Rotation", Controls.AddLabel($"Crew.Rotation", 20, y++));
        Controls.AddLabel($"Morale:", 2, y);
        Binds.Add("Morale", Controls.AddLabel($"Crew.Morale", 20, y++));
        Controls.AddLabel($"Wages:", 2, y);
        Binds.Add("Wages", Controls.AddLabel($"Crew.Wages", 20, y++));
    }
}
