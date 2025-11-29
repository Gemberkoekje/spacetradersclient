using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class EngineWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Engine? Engine { get; set; }
    private ShipService ShipService { get; init; }

    public EngineWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var engine = data.First(s => s.Symbol == Symbol).Engine;
        if (Engine is not null && Engine == engine)
            return Task.CompletedTask;
        Title = $"Engine for ship {Symbol}";
        Engine = engine;
        DrawContent();
        return Task.CompletedTask;
    }

    public void SetSymbol(string symbol, string? _)
    {
        Symbol = symbol;
        LoadData(ShipService.GetShips().ToArray());
    }

    private void DrawContent()
    {
        Clean();
        if (Engine is null)
        {
            Controls.AddLabel($"Engine data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Engine.Name}", 2, y++);
        Controls.AddLabel($"Condition: {Engine.Condition}", 2, y++);
        Controls.AddLabel($"Integrity: {Engine.Integrity}", 2, y++);
        Controls.AddLabel($"Speed: {Engine.Speed}", 2, y++);
        Controls.AddLabel($"Power Requirements: {Engine.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Engine.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Engine.Requirements.Slots}", 2, y++);
        Controls.AddLabel($"Quality: {Engine.Quality}", 2, y++);
        ResizeAndRedraw();
    }
}
