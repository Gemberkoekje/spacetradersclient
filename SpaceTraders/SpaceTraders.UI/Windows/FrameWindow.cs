using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class FrameWindow : ClosableWindow, ICanSetSymbols
{
    private Frame? Frame { get; set; }

    private ModuleService ModuleService { get; init; }

    public FrameWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = moduleService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var frame = ModuleService.GetFrames().GetValueOrDefault(Enum.Parse<FrameSymbol>(symbols[0]));
        if (frame != null)
        {
            Title = $"{frame.Name}";
        }
        Frame = frame;
        DrawContent();
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
