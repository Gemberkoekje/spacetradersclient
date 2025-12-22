using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class MountWindow : ClosableWindow, ICanSetSymbols
{
    private Mount? Mount { get; set; }

    private ModuleService ModuleService { get; init; }

    public MountWindow(RootScreen rootScreen, ModuleService mountService)
        : base(rootScreen, 52, 20)
    {
        ModuleService = mountService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        var mount = ModuleService.GetMounts().GetValueOrDefault(Enum.Parse<MountSymbol>(symbols[0]));
        if (mount == null)
            return;
        Title = $"{mount.Name}";
        Mount = mount;
        Binds["Name"].SetData([$"{Mount.Name}"]);
        Binds["Strength"].SetData([$"{Mount.Strength}"]);
        Binds["Deposits"].SetData([.. Mount.Deposits.Select(d => $"{d}")]);
        Binds["Power"].SetData([$"{Mount.Requirements.Power}"]);
        Binds["Crew"].SetData([$"{Mount.Requirements.Crew}"]);
        Binds["Slots"].SetData([$"{Mount.Requirements.Slots}"]);
        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Mount.Name", 22, y++));
        Controls.AddLabel($"Strength:", 2, y);
        Binds.Add("Strength", Controls.AddLabel($"Mount.Strength", 22, y++));

        Controls.AddLabel($"Power Requirements:", 2, y);
        Binds.Add("Power", Controls.AddLabel($"Mount.Requirements.Power", 22, y++));
        Controls.AddLabel($"Crew Requirements:", 2, y);
        Binds.Add("Crew", Controls.AddLabel($"Mount.Requirements.Crew", 22, y++));
        Controls.AddLabel($"Slots Requirements:", 2, y);
        Binds.Add("Slots", Controls.AddLabel($"Mount.Requirements.Slots", 22, y++));
        y++;
        Controls.AddLabel($"Deposits:", 2, y);
        Binds.Add("Deposits", Controls.AddListbox($"Deposits", 22, y, 80, 10));
        y += 10;

    }
}
