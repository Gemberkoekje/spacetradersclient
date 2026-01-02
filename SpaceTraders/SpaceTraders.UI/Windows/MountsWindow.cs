using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class MountsWindow : DataBoundWindowWithContext<ImmutableArray<Mount>, ShipContext>
{
    private readonly ShipService _shipService;
    private CustomListBox<MountListValue>? _mountsListBox;

    public MountsWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override ImmutableArray<Mount> FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship)?.Mounts ?? [];

    protected override void BindData(ImmutableArray<Mount> data)
    {
        Title = $"Mounts for ship {Context.Ship}";
        _mountsListBox?.SetCustomData([.. data.Select(mount => new MountListValue(mount))]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        _mountsListBox = Controls.AddListbox<MountListValue>($"Mounts", 2, y, 80, 10);
        Binds.Add("Mounts", _mountsListBox);
        y += 10;
        Controls.AddButton("Show Mount", 2, y, (_, _) => OpenMount());
    }

    private void OpenMount()
    {
        if (_mountsListBox?.GetSelectedItem() is MountListValue mountListValue)
        {
            RootScreen.ShowWindow<MountWindow, MountContext>(new (mountListValue.Mount.Symbol));
        }
    }

    private sealed record MountListValue(Mount Mount)
    {
        public override string ToString() => $"{Mount.Name} (Strength: {Mount.Strength}{(Mount.Deposits.Any() ? $", Deposits: {Mount.Deposits.Count}" : string.Empty)})";
    }
}
