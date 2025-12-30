using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

internal sealed class ModuleWindow : DataBoundWindowWithSymbols<Module>
{
    private readonly ModuleService _moduleService;

    public ModuleWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        _moduleService = moduleService;

        Initialize();
    }

    protected override Module? FetchData()
    {
        if (string.IsNullOrEmpty(Symbol)) return null;
        return _moduleService.GetModules().GetValueOrDefault(Enum.Parse<ModuleSymbol>(Symbol));
    }

    protected override void BindData(Module data)
    {
        Title = $"{data.Name}";
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Capacity"].SetData([$"{data.Capacity}"]);
        Binds["Range"].SetData([$"{data.Range}"]);
        Binds["Power"].SetData([$"{data.Requirements.Power}"]);
        Binds["Crew"].SetData([$"{data.Requirements.Crew}"]);
        Binds["Slots"].SetData([$"{data.Requirements.Slots}"]);
    }

    protected override void DrawContent()
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
        Binds.Add("Slots", Controls.AddLabel($"Module.Requirements.Slots", 22, y));
    }
}
