using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class ModuleWindow : ClosableWindow, ICanSetSymbols
{
    private Module? Module { get; set; }

    private ModuleService ModuleService { get; init; }

    public ModuleWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = moduleService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var module = ModuleService.GetModules().GetValueOrDefault(Enum.Parse<ModuleSymbol>(symbols[0]));
        if (module != null)
        {
            Title = $"{module.Name}";
        }
        Module = module;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Module is null)
        {
            Controls.AddLabel($"Module data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Name: {Module.Name}", 2, y++);
        Controls.AddLabel($"Capacity: {Module.Capacity}", 2, y++);
        Controls.AddLabel($"Range: {Module.Range}", 2, y++);
        Controls.AddLabel($"Power Requirements: {Module.Requirements.Power}", 2, y++);
        Controls.AddLabel($"Crew Requirements: {Module.Requirements.Crew}", 2, y++);
        Controls.AddLabel($"Slots Requirements: {Module.Requirements.Slots}", 2, y++);
        ResizeAndRedraw();
    }
}
