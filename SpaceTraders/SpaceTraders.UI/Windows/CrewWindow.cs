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
        if (Crew is null)
        {
            Controls.AddLabel($"Crew data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Current Crew: {Crew.Current}", 2, y++);
        Controls.AddLabel($"Crew Capacity: {Crew.Capacity}", 2, y++);
        Controls.AddLabel($"Rotation: {Crew.Rotation}", 2, y++);
        Controls.AddLabel($"Morale: {Crew.Morale}", 2, y++);
        Controls.AddLabel($"Wages: {Crew.Wages}", 2, y++);
        ResizeAndRedraw();
    }
}
