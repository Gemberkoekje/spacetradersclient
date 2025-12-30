using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class MountWindow : DataBoundWindowWithSymbols<Mount>
{
    private readonly ModuleService _moduleService;

    public MountWindow(RootScreen rootScreen, ModuleService moduleService)
        : base(rootScreen, 52, 20)
    {
        _moduleService = moduleService;

        Initialize();
    }

    protected override Mount? FetchData()
    {
        if (string.IsNullOrEmpty(Symbol)) return null;
        return _moduleService.GetMounts().GetValueOrDefault(Enum.Parse<MountSymbol>(Symbol));
    }

    protected override void BindData(Mount data)
    {
        Title = $"{data.Name}";
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Strength"].SetData([$"{data.Strength}"]);
        Binds["Deposits"].SetData([.. data.Deposits.Select(d => $"{d}")]);
        Binds["Power"].SetData([$"{data.Requirements.Power}"]);
        Binds["Crew"].SetData([$"{data.Requirements.Crew}"]);
        Binds["Slots"].SetData([$"{data.Requirements.Slots}"]);
    }

    protected override void DrawContent()
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
    }
}
