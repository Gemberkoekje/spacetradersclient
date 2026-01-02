using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class ReactorWindow : DataBoundWindowWithContext<Reactor, ReactorContext>
{
    private readonly ModuleService _moduleService;

    public ReactorWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        _moduleService = moduleService;

        Initialize();
    }

    protected override Reactor? FetchData()
    {
        return _moduleService.GetReactors().GetValueOrDefault(Context.Reactor);
    }

    protected override void BindData(Reactor data)
    {
        Title = $"{data.Name}";
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Condition"].SetData([$"{data.Condition}"]);
        Binds["Integrity"].SetData([$"{data.Integrity}"]);
        Binds["PowerOutput"].SetData([$"{data.PowerOutput}"]);
        Binds["Requirements.Power"].SetData([$"{data.Requirements.Power}"]);
        Binds["Requirements.Crew"].SetData([$"{data.Requirements.Crew}"]);
        Binds["Requirements.Slots"].SetData([$"{data.Requirements.Slots}"]);
        Binds["Quality"].SetData([$"{data.Quality}"]);
    }

    protected override void DrawContent()
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
        Binds.Add("Quality", Controls.AddLabel($"Reactor.Quality", 22, y));
    }
}
