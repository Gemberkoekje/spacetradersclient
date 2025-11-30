using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class ReactorWindow : ClosableWindow, ICanSetSymbols
{
    private Reactor? Reactor { get; set; }

    private ModuleService ModuleService { get; init; }

    public ReactorWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = moduleService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var reactor = ModuleService.GetReactors().GetValueOrDefault(Enum.Parse<ReactorSymbol>(symbols[0]));
        if (reactor != null)
        {
            Title = $"{reactor.Name}";
        }
        Reactor = reactor;
        DrawContent();
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
