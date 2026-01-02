using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardMountsWindow : DataBoundWindowWithContext<ImmutableArray<Mount>, ShipyardShipContext>
{
    private readonly ShipyardService _shipyardService;
    private CustomListBox? _mountsListBox;

    public ShipyardMountsWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Shipyard>>>(
            handler => shipyardService.Updated += handler,
            handler => shipyardService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override ImmutableArray<Mount> FetchData()
    {
        var shipyards = _shipyardService.GetShipyards().GetValueOrDefault(Context.System);
        if (shipyards.IsDefault) return [];
        var shipyard = shipyards.FirstOrDefault(s => s.Symbol == Context.Waypoint);
        var ship = shipyard?.Ships.FirstOrDefault(s => s.Type == Context.ShipType);
        return ship?.Mounts ?? [];
    }

    protected override void BindData(ImmutableArray<Mount> data)
    {
        Title = $"Mounts for {Context.ShipType} in {Context.Waypoint} shipyard";
        Binds["Mounts"].SetData([.. data.Select(mount => $"{mount.Name} (Strength: {mount.Strength}{(mount.Deposits.Any() ? $", Deposits: {mount.Deposits.Count}" : string.Empty)})")]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        _mountsListBox = Controls.AddListbox($"Mounts", 2, y, 80, 10);
        Binds.Add("Mounts", _mountsListBox);
        y += 10;
        Controls.AddButton("Show Mount", 2, y, (_, _) => OpenMount());
    }

    private void OpenMount()
    {
        if (_mountsListBox is null || CurrentData.IsDefault)
            return;
        var listbox = _mountsListBox;
        if (listbox.SelectedIndex is int index and >= 0 && index < CurrentData.Length)
        {
            var mount = CurrentData[index];
            RootScreen.ShowWindow<MountWindow, MountContext>(new (mount.Symbol));
        }
    }
}
