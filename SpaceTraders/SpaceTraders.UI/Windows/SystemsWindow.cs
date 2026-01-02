using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemsWindow : DataBoundWindowNoSymbols<ImmutableArray<SystemWaypoint>>
{
    private readonly ShipService _shipService;
    private readonly SystemService _systemService;

    private ImmutableArray<Ship> _ships = [];
    private CustomListBox<SystemListValue>? _systemsListBox;

    public SystemsWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        _shipService = shipService;
        _systemService = systemService;
        Title = "Known Systems";

        SubscribeToEvent<ImmutableArray<SystemWaypoint>>(
            handler => systemService.Updated += handler,
            handler => systemService.Updated -= handler,
            OnServiceUpdatedSync);

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnShipsUpdated);

        Initialize(refreshImmediately: true);
    }

    protected override ImmutableArray<SystemWaypoint> FetchData() =>
        _systemService.GetSystems();

    protected override void BindData(ImmutableArray<SystemWaypoint> data)
    {
        _ships = _shipService.GetShips();
        _systemsListBox?.SetCustomData([.. data.Select(s => new SystemListValue(s, _ships.Count(sh => sh.Navigation.SystemSymbol == s.Symbol)))]);
    }

    private Task OnShipsUpdated(ImmutableArray<Ship> ships)
    {
        _ships = ships;
        RefreshData();
        return Task.CompletedTask;
    }

    protected override void DrawContent()
    {
        var y = 2;
        _systemsListBox = Controls.AddListbox<SystemListValue>($"Systems", 2, y, 80, 10);
        Binds.Add("Systems", _systemsListBox);
        y += 10;
        Controls.AddButton("Show System", 2, y, (_, _) => OpenSystem());
    }

    private void OpenSystem()
    {
        if (_systemsListBox?.GetSelectedItem() is SystemListValue waypoint)
        {
            RootScreen.ShowWindow<SystemDataWindow, SystemContext>(new (waypoint.System.Symbol));
        }
    }

    private sealed class SystemListValue(SystemWaypoint system, int shipCount) : ColoredString(GetDisplayText(system, shipCount), GetForegroundColor(shipCount), Color.Transparent)
    {
        public SystemWaypoint System => system;

        private static string GetDisplayText(SystemWaypoint system, int shipCount)
        {
            return $"{system.Symbol} ({system.SystemType}) - Ships: {shipCount}";
        }

        private static Color GetForegroundColor(int shipCount)
        {
            if (shipCount > 0)
            {
                return Color.Cyan;
            }
            return Color.AnsiWhite;
        }
    }
}
