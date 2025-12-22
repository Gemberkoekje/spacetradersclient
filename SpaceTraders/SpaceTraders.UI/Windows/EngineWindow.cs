using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class EngineWindow : ClosableWindow, ICanSetSymbols
{
    private Engine? Engine { get; set; }

    private ModuleService ModuleService { get; init; }

    public EngineWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = moduleService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var engine = ModuleService.GetEngines().GetValueOrDefault(Enum.Parse<EngineSymbol>(symbols[0]));
        if (engine != null)
        {
            Title = $"{engine.Name}";
            Engine = engine;
            Binds["Name"].SetData([$"{Engine.Name}"]);
            Binds["Condition"].SetData([$"{Engine.Condition}"]);
            Binds["Integrity"].SetData([$"{Engine.Integrity}"]);
            Binds["Speed"].SetData([$"{Engine.Speed}"]);
            Binds["Requirements.Power"].SetData([$"{Engine.Requirements.Power}"]);
            Binds["Requirements.Crew"].SetData([$"{Engine.Requirements.Crew}"]);
            Binds["Requirements.Slots"].SetData([$"{Engine.Requirements.Slots}"]);
            Binds["Quality"].SetData([$"{Engine.Quality}"]);
            ResizeAndRedraw();
        }
    }

    private void DrawContent()
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
        Binds.Add("Quality", Controls.AddLabel($"Engine.Quality", 25, y++));
    }
}
