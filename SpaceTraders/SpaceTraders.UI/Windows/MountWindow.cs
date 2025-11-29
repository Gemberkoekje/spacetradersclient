using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class MountWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private Mount? Mount { get; set; }
    private ShipService ShipService { get; init; }

    public MountWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var mount = data.First(s => s.Symbol == ParentSymbol).Mounts.First(m => m.Symbol.ToString() == Symbol);
        if (Mount is not null && Mount == mount)
            return Task.CompletedTask;
        Title = $"Mount for ship {Symbol}";
        Mount = mount;
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
        if (Mount is null)
        {
            Controls.AddLabel($"Mount data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Mount.Name}", 2, y++);
        Controls.AddLabel($"Strength: {Mount.Strength}", 2, y++);
        Controls.AddLabel($"Deposits:", 2, y++);
        foreach (var deposit in Mount.Deposits)
        {
            Controls.AddLabel($"- {deposit}", 4, y++);
        }
        Controls.AddLabel($"Power Requirements: {Mount.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Mount.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Mount.Requirements.Slots}", 2, y++);
        ResizeAndRedraw();
    }
}
