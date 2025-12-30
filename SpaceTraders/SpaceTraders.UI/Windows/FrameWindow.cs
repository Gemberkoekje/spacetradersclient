using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class FrameWindow : DataBoundWindowWithSymbols<Frame>
{
    private readonly ModuleService _moduleService;

    public FrameWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        _moduleService = moduleService;

        Initialize();
    }

    protected override Frame? FetchData()
    {
        if (string.IsNullOrEmpty(Symbol)) return null;
        return _moduleService.GetFrames().GetValueOrDefault(Enum.Parse<FrameSymbol>(Symbol));
    }

    protected override void BindData(Frame data)
    {
        Title = $"{data.Name}";
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Condition"].SetData([$"{data.Condition}"]);
        Binds["Integrity"].SetData([$"{data.Integrity}"]);
        Binds["ModuleSlots"].SetData([$"{data.ModuleSlots}"]);
        Binds["MountingPoints"].SetData([$"{data.MountingPoints}"]);
        Binds["FuelCapacity"].SetData([$"{data.FuelCapacity}"]);
        Binds["Requirements.Power"].SetData([$"{data.Requirements.Power}"]);
        Binds["Requirements.Crew"].SetData([$"{data.Requirements.Crew}"]);
        Binds["Requirements.Slots"].SetData([$"{data.Requirements.Slots}"]);
    }

    protected override void DrawContent()
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
        Binds.Add("Requirements.Slots", Controls.AddLabel($"Frame.Requirements.Slots", 25, y));
    }
}
