using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class EngineWindow : DataBoundWindowWithSymbols<Engine>
{
    private readonly ModuleService _moduleService;

    public EngineWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        _moduleService = moduleService;

        Initialize();
    }

    protected override Engine? FetchData()
    {
        if (string.IsNullOrEmpty(Symbol)) return null;
        return _moduleService.GetEngines().GetValueOrDefault(Enum.Parse<EngineSymbol>(Symbol));
    }

    protected override void BindData(Engine data)
    {
        Title = $"{data.Name}";
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Condition"].SetData([$"{data.Condition}"]);
        Binds["Integrity"].SetData([$"{data.Integrity}"]);
        Binds["Speed"].SetData([$"{data.Speed}"]);
        Binds["Requirements.Power"].SetData([$"{data.Requirements.Power}"]);
        Binds["Requirements.Crew"].SetData([$"{data.Requirements.Crew}"]);
        Binds["Requirements.Slots"].SetData([$"{data.Requirements.Slots}"]);
        Binds["Quality"].SetData([$"{data.Quality}"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Engine.Name", 25, y++));
        Controls.AddLabel($"Condition:", 2, y);
        Binds.Add("Condition", Controls.AddLabel($"Engine.Condition", 25, y++));
        Controls.AddLabel($"Integrity:", 2, y);
        Binds.Add("Integrity", Controls.AddLabel($"Engine.Integrity", 25, y++));
        Controls.AddLabel($"Speed:", 2, y);
        Binds.Add("Speed", Controls.AddLabel($"Engine.Speed", 25, y++));
        Controls.AddLabel($"Power Requirements:", 2, y);
        Binds.Add("Requirements.Power", Controls.AddLabel($"Engine.Requirements.Power", 25, y++));
        Controls.AddLabel($"Crew Requirements:", 2, y);
        Binds.Add("Requirements.Crew", Controls.AddLabel($"Engine.Requirements.Crew", 25, y++));
        Controls.AddLabel($"Slots Requirements:", 2, y);
        Binds.Add("Requirements.Slots", Controls.AddLabel($"Engine.Requirements.Slots", 25, y++));
        Controls.AddLabel($"Quality:", 2, y);
        Binds.Add("Quality", Controls.AddLabel($"Engine.Quality", 25, y));
    }
}
