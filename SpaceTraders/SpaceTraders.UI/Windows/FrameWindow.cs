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
            Frame = frame;
            Binds["Name"].SetData([$"{Frame.Name}"]);
            Binds["Condition"].SetData([$"{Frame.Condition}"]);
            Binds["Integrity"].SetData([$"{Frame.Integrity}"]);
            Binds["ModuleSlots"].SetData([$"{Frame.ModuleSlots}"]);
            Binds["MountingPoints"].SetData([$"{Frame.MountingPoints}"]);
            Binds["FuelCapacity"].SetData([$"{Frame.FuelCapacity}"]);
            Binds["Requirements.Power"].SetData([$"{Frame.Requirements.Power}"]);
            Binds["Requirements.Crew"].SetData([$"{Frame.Requirements.Crew}"]);
            Binds["Requirements.Slots"].SetData([$"{Frame.Requirements.Slots}"]);
            ResizeAndRedraw();
        }
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Frame.Name", 25, y++));
        Controls.AddLabel($"Condition:", 2, y);
        Binds.Add("Condition", Controls.AddLabel($"Frame.Condition", 25, y++));
        Controls.AddLabel($"Integrity:", 2, y);
        Binds.Add("Integrity", Controls.AddLabel($"Frame.Integrity", 25, y++));
        Controls.AddLabel($"Module slots:", 2, y);
        Binds.Add("ModuleSlots", Controls.AddLabel($"Frame.ModuleSlots", 25, y++));
        Controls.AddLabel($"Mounting points:", 2, y);
        Binds.Add("MountingPoints", Controls.AddLabel($"Frame.MountingPoints", 25, y++));
        Controls.AddLabel($"Fuel capacity:", 2, y);
        Binds.Add("FuelCapacity", Controls.AddLabel($"Frame.FuelCapacity", 25, y++));
        Controls.AddLabel($"Power Requirements:", 2, y);
        Binds.Add("Requirements.Power", Controls.AddLabel($"Frame.Requirements.Power", 25, y++));
        Controls.AddLabel($"Crew Requirements:", 2, y);
        Binds.Add("Requirements.Crew", Controls.AddLabel($"Frame.Requirements.Crew", 25, y++));
        Controls.AddLabel($"Slots Requirements:", 2, y);
        Binds.Add("Requirements.Slots", Controls.AddLabel($"Frame.Requirements.Slots", 25, y++));
    }
}
