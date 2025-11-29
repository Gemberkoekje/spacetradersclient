using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class FrameWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Frame? Frame { get; set; }
    private ShipService ShipService { get; init; }

    public FrameWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        shipService.Updated += LoadData;
        ShipService = shipService;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        var frame = data.First(s => s.Symbol == Symbol).Frame;
        if (Frame is not null && Frame == frame)
            return Task.CompletedTask;
        Title = $"Frame for ship {Symbol}";
        Frame = frame;
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
        if (Frame is null)
        {
            Controls.AddLabel($"Frame data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Frame.Name}", 2, y++);
        Controls.AddLabel($"Condition: {Frame.Condition}", 2, y++);
        Controls.AddLabel($"Integrity: {Frame.Integrity}", 2, y++);
        Controls.AddLabel($"ModuleSlots: {Frame.ModuleSlots}", 2, y++);
        Controls.AddLabel($"MountingPoints: {Frame.MountingPoints}", 2, y++);
        Controls.AddLabel($"FuelCapacity: {Frame.FuelCapacity}", 2, y++);
        Controls.AddLabel($"Power Requirements: {Frame.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Frame.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Frame.Requirements.Slots}", 2, y++);
        ResizeAndRedraw();
    }
}
