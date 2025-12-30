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
        if (reactor == null)
        {
            return;
        }
        Title = $"{reactor.Name}";
        Reactor = reactor;
        Binds["Name"].SetData([$"{Reactor.Name}"]);
        Binds["Condition"].SetData([$"{Reactor.Condition}"]);
        Binds["Integrity"].SetData([$"{Reactor.Integrity}"]);
        Binds["PowerOutput"].SetData([$"{Reactor.PowerOutput}"]);
        Binds["Requirements.Power"].SetData([$"{Reactor.Requirements.Power}"]);
        Binds["Requirements.Crew"].SetData([$"{Reactor.Requirements.Crew}"]);
        Binds["Requirements.Slots"].SetData([$"{Reactor.Requirements.Slots}"]);
        Binds["Quality"].SetData([$"{Reactor.Quality}"]);

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Reactor.Name", 22, y++));
        Controls.AddLabel($"Condition:", 2, y);
        Binds.Add("Condition", Controls.AddLabel($"Reactor.Condition", 22, y++));
        Controls.AddLabel($"Integrity:", 2, y);
        Binds.Add("Integrity", Controls.AddLabel($"Reactor.Integrity", 22, y++));
        Controls.AddLabel($"Power Output:", 2, y);
        Binds.Add("PowerOutput", Controls.AddLabel($"Reactor.PowerOutput", 22, y++));
        Controls.AddLabel($"Power Requirements:", 2, y);
        Binds.Add("Requirements.Power", Controls.AddLabel($"Reactor.Requirements.Power", 22, y++));
        Controls.AddLabel($"Crew Requirements:", 2, y);
        Binds.Add("Requirements.Crew", Controls.AddLabel($"Reactor.Requirements.Crew", 22, y++));
        Controls.AddLabel($"Slots Requirements:", 2, y);
        Binds.Add("Requirements.Slots", Controls.AddLabel($"Reactor.Requirements.Slots", 22, y++));
        Controls.AddLabel($"Quality:", 2, y);
        Binds.Add("Quality", Controls.AddLabel($"Reactor.Quality", 22, y++));
    }
}
