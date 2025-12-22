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
        if (module == null)
            return;
        Title = $"{module.Name}";
        Module = module;
        Binds["Name"].SetData([$"{Module.Name}"]);
        Binds["Capacity"].SetData([$"{Module.Capacity}"]);
        Binds["Range"].SetData([$"{Module.Range}"]);
        Binds["Power"].SetData([$"{Module.Requirements.Power}"]);
        Binds["Crew"].SetData([$"{Module.Requirements.Crew}"]);
        Binds["Slots"].SetData([$"{Module.Requirements.Slots}"]);
        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Module.Name", 22, y++));
        Controls.AddLabel($"Capacity:", 2, y);
        Binds.Add("Capacity", Controls.AddLabel($"Module.Capacity", 22, y++));
        Controls.AddLabel($"Range:", 2, y);
        Binds.Add("Range", Controls.AddLabel($"Module.Range", 22, y++));
        Controls.AddLabel($"Power Requirements:", 2, y);
        Binds.Add("Power", Controls.AddLabel($"Module.Requirements.Power", 22, y++));
        Controls.AddLabel($"Crew Requirements:", 2, y);
        Binds.Add("Crew", Controls.AddLabel($"Module.Requirements.Crew", 22, y++));
        Controls.AddLabel($"Slots Requirements:", 2, y);
        Binds.Add("Slots", Controls.AddLabel($"Module.Requirements.Slots", 22, y++));
    }
}
