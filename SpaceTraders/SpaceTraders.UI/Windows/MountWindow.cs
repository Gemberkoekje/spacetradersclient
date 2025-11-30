using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class MountWindow : ClosableWindow, ICanSetSymbols
{
    private Mount? Mount { get; set; }

    private ModuleService ModuleService { get; init; }

    public MountWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = moduleService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var mount = ModuleService.GetMounts().GetValueOrDefault(Enum.Parse<MountSymbol>(symbols[0]));
        if (mount != null)
        {
            Title = $"{mount.Name}";
        }
        Mount = mount;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Mount is null)
        {
            Controls.AddLabel($"Mount data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Mount.Name}", 2, y++);
        Controls.AddLabel($"Strength: {Mount.Strength}", 2, y++);
        Controls.AddLabel($"Deposits:", 2, y++);
        foreach (var deposit in Mount.Deposits)
        {
            Controls.AddLabel($"- {deposit}", 4, y++);
        }
        Controls.AddLabel($"Power Requirements: {Mount.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Mount.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Mount.Requirements.Slots}", 2, y++);
        ResizeAndRedraw();
    }
}
