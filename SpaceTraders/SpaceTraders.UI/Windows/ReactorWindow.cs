using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ReactorWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Reactor? Reactor { get; set; }
    private ShipService ShipService { get; init; }

    public ReactorWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var reactor = data.First(s => s.Symbol == Symbol).Reactor;
        if (Reactor is not null && Reactor == reactor)
            return Task.CompletedTask;
        Title = $"Reactor for ship {Symbol}";
        Reactor = reactor;
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
        if (Reactor is null)
        {
            Controls.AddLabel($"Reactor data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Reactor.Name}", 2, y++);
        Controls.AddLabel($"Condition: {Reactor.Condition}", 2, y++);
        Controls.AddLabel($"Integrity: {Reactor.Integrity}", 2, y++);
        Controls.AddLabel($"PowerOutput: {Reactor.PowerOutput}", 2, y++);
        Controls.AddLabel($"Power Requirements: {Reactor.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Reactor.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Reactor.Requirements.Slots}", 2, y++);
        Controls.AddLabel($"Quality: {Reactor.Quality}", 2, y++);
        ResizeAndRedraw();
    }
}
