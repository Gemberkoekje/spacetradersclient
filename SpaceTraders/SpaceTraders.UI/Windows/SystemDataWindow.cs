using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : DataBoundWindowWithContext<SystemWaypoint, SystemContext>
{
    private readonly ShipService _shipService;
    private readonly SystemService _systemService;
    private readonly WaypointService _waypointService;

    private CustomListBox<WaypointListValue>? _waypointListBox;

    public SystemDataWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        _shipService = shipService;
        _systemService = systemService;
        _waypointService = waypointService;

        SubscribeToEvent<ImmutableArray<SystemWaypoint>>(
            handler => systemService.Updated += handler,
            handler => systemService.Updated -= handler,
            OnServiceUpdatedSync);

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>>>(
            handler => waypointService.Updated += handler,
            handler => waypointService.Updated -= handler,
            OnWaypointsUpdated);

        Initialize();
    }

    protected override SystemWaypoint? FetchData() =>
        _systemService.GetSystems().FirstOrDefault(d => d.Symbol == Context.System);

    protected override void BindData(SystemWaypoint data)
    {
        Title = $"System {data.Name}";

        // Update waypoints and ships
        var waypoints = _waypointService.GetWaypoints().GetValueOrDefault(Context.System);
        var ships = _shipService.GetShips().Where(d => d.Navigation.SystemSymbol == data.Symbol).ToImmutableArray();

        Binds["Symbol"].SetData([$"{data.Symbol}"]);
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Constellation"].SetData([$"{data.Constellation}"]);
        Binds["Sector"].SetData([$"{data.SectorSymbol}"]);
        Binds["Type"].SetData([$"{data.SystemType}"]);
        Binds["Position"].SetData([$"{data.X:0,000}, {data.Y:0,000}"]);
        Binds["Waypoints"].SetData([$"{waypoints.Length}"]);
        Binds["Factions"].SetData([$"{data.Factions.Length}"]);

        var waypointdata = new List<WaypointListValue>();

        foreach (var wp in waypoints.Where(wp => wp.Type != Core.Enums.WaypointType.Asteroid).OrderBy(w => w.Symbol))
        {
            waypointdata.Add(new WaypointListValue(wp, null, ships.Any(s => s.Navigation.WaypointSymbol == wp.Symbol)));
            foreach (var ship in ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol))
            {
                waypointdata.Add(new WaypointListValue(wp, ship, true));
            }
        }
        foreach (var wp in waypoints.Where(wp => wp.Type == Core.Enums.WaypointType.Asteroid).OrderBy(w => w.Symbol))
        {
            waypointdata.Add(new WaypointListValue(wp, null, ships.Any(s => s.Navigation.WaypointSymbol == wp.Symbol)));
            foreach (var ship in ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol))
            {
                waypointdata.Add(new WaypointListValue(wp, ship, true));
            }
        }
        _waypointListBox?.SetCustomData([.. waypointdata]);
    }

    private Task OnWaypointsUpdated(ImmutableDictionary<SystemSymbol, ImmutableArray<Waypoint>> _)
    {
        RefreshData();
        return Task.CompletedTask;
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"System.Symbol", 21, y++));
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"System.Name", 21, y++));
        Controls.AddLabel($"Constellation:", 2, y);
        Binds.Add("Constellation", Controls.AddLabel($"System.Constellation", 21, y++));
        Controls.AddLabel($"Sector:", 2, y);
        Binds.Add("Sector", Controls.AddLabel($"System.SectorSymbol", 21, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"System.SystemType", 21, y++));
        Controls.AddLabel($"Position:", 2, y);
        Binds.Add("Position", Controls.AddLabel($"System.X System.Y", 21, y++));
        Controls.AddLabel($"Waypoints:", 2, y);
        Binds.Add("Waypoints", Controls.AddLabel($"System.Waypoints.Count", 21, y++));
        Controls.AddLabel($"Factions:", 2, y);
        Binds.Add("Factions", Controls.AddLabel($"System.Factions.Count", 21, y++));
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow, SystemContext>(Context));
        y++;

        Controls.AddLabel($"Markets & Shipyards:", 2, y++);
        _waypointListBox = Controls.AddListbox<WaypointListValue>($"WaypointList", 2, y, 80, 25);
        Binds.Add("WaypointList", _waypointListBox);
        y += 25;
        Controls.AddButton("Show Waypoint", 2, y, (_, _) => OpenWaypoint());
    }

    private void OpenWaypoint()
    {
        if (_waypointListBox?.GetSelectedItem() is WaypointListValue waypoint)
        {
            if (waypoint.Ship is not null)
            {
                RootScreen.ShowWindow<ShipWindow, ShipContext>(new (waypoint.Ship.Symbol));
                return;
            }
            RootScreen.ShowWindow<WaypointWindow, WaypointContext>(new (waypoint.Waypoint.Symbol, waypoint.Waypoint.SystemSymbol));
        }
    }

    private sealed class WaypointListValue(Waypoint wp, Ship? ship, bool hasShipsDocked) : ColoredString(GetDisplayText(wp, ship), GetForegroundColor(wp, ship, hasShipsDocked), Color.Transparent)
    {
        public Waypoint Waypoint => wp;

        public Ship? Ship => ship;

        private static string GetDisplayText(Waypoint wp, Ship? ship)
        {
            int indent = 0;
            if (ship != null)
            {
                indent += 1;
            }
            if (wp.Orbits != WaypointSymbol.Empty)
            {
                indent += 1;
            }
            var substring = indent > 0 ? $"{new string(' ', indent - 1)}- " : string.Empty;
            if (ship != null)
            {
                return $"{substring}{ship.Symbol} ({ship.Registration.Role})";
            }
            return $"{substring}{wp.Symbol} ({wp.Type}){(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace) ? " Marketplace" : string.Empty)}{(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) ? " & Shipyard" : string.Empty)}";
        }

        private static Color GetForegroundColor(Waypoint wp, Ship? ship, bool hasShipsDocked)
        {
            if (ship != null)
            {
                return Color.WhiteSmoke;
            }
            if (hasShipsDocked)
            {
                return Color.Cyan;
            }
            if (wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace))
            {
                return Color.MediumSeaGreen;
            }
            if (wp.Type == Core.Enums.WaypointType.Asteroid)
            {
                return Color.Gray;
            }
            return Color.AnsiWhite;
        }
    }
}
