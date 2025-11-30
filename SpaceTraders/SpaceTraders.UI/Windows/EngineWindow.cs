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
        }
        Engine = engine;
        DrawContent();
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
