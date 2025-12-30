using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemMapWindow : DataBoundWindowWithSymbols<SystemWaypoint>
{
    private readonly SystemService _systemService;
    private readonly WaypointService _waypointService;
    private readonly ShipService _shipService;

    private ImmutableArray<Waypoint> _waypoints = [];
    private ImmutableArray<Ship> _ships = [];
    private readonly Dictionary<string, CustomLabel> _waypointBinds = [];

    public SystemMapWindow(RootScreen rootScreen, SystemService systemService, WaypointService waypointService, ShipService shipService)
        : base(rootScreen, 45, 30)
    {
        _systemService = systemService;
        _waypointService = waypointService;
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<SystemWaypoint>>(
            handler => systemService.Updated += handler,
            handler => systemService.Updated -= handler,
            OnServiceUpdatedSync);

        SubscribeToEvent<ImmutableDictionary<string, ImmutableArray<Waypoint>>>(
            handler => waypointService.Updated += handler,
            handler => waypointService.Updated -= handler,
            OnWaypointsUpdated);

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnShipsUpdated);

        Initialize();
    }

    protected override SystemWaypoint? FetchData() =>
        _systemService.GetSystems().FirstOrDefault(d => d.Symbol == Symbol);

    protected override void BindData(SystemWaypoint data)
    {
        Title = $"System {data.Symbol}";

        // Update waypoints and ships
        _waypoints = _waypointService.GetWaypoints().GetValueOrDefault(Symbol);
        _ships = [.. _shipService.GetShips().Where(d => d.Navigation.SystemSymbol == Symbol)];

        if (_waypoints.IsDefault || _waypoints.Length == 0)
            return;

        var (minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY) = ComputeExtents();

        MapSun(data, minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        MapWaypoints(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        MapShips(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
    }

    private Task OnWaypointsUpdated(ImmutableDictionary<string, ImmutableArray<Waypoint>> _)
    {
        RefreshData();
        return Task.CompletedTask;
    }

    private Task OnShipsUpdated(ImmutableArray<Ship> _)
    {
        RefreshData();
        return Task.CompletedTask;
    }

    private (int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY) ComputeExtents()
    {
        int minX = _waypoints.Min(w => w.X);
        int minY = _waypoints.Min(w => w.Y);
        int maxX = _waypoints.Max(w => w.X);
        int maxY = _waypoints.Max(w => w.Y);

        int desiredWidth = (int)(50.0 * ((float)SadConsole.Game.Instance.DefaultFont.GlyphHeight / (float)SadConsole.Game.Instance.DefaultFont.GlyphWidth));
        const int desiredHeight = 50;
        int rangeX = maxX - minX;
        int rangeY = maxY - minY;

        bool singleX = rangeX == 0;
        bool singleY = rangeY == 0;

        float scaleX = singleX ? 0f : (desiredWidth - 1f) / rangeX;
        float scaleY = singleY ? 0f : (desiredHeight - 1f) / rangeY;

        float offsetX = 2f;
        float offsetY = 2f;

        if (singleX) offsetX = desiredWidth / 2f;
        if (singleY) offsetY = desiredHeight / 2f;

        return (minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
    }

    private void MapShips(int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        foreach (var ship in _ships.Where(s => s.Navigation.Status == ShipNavStatus.InTransit))
        {
            if (ship.Position is null) continue;

            float translatedX = ship.Position.Value.X - minX;
            float translatedY = ship.Position.Value.Y - minY;

            int canvasX = singleX
                ? (int)Math.Round(offsetX)
                : (int)Math.Round(offsetX + translatedX * scaleX);

            int canvasY = singleY
                ? (int)Math.Round(offsetY)
                : (int)Math.Round(offsetY + translatedY * scaleY);

            canvasX = Math.Clamp(canvasX, 0, desiredWidth - 1);
            canvasY = Math.Clamp(canvasY, 0, desiredHeight - 1);

            var currentLabel = Binds.GetValueOrDefault(ship.Symbol);

            if (currentLabel is null)
            {
                Binds.Add(ship.Symbol, Controls.AddLabel($"{ShipSymbol(ship.Direction)}", canvasX, canvasY, Color.White));
            }
            else
            {
                Binds[ship.Symbol].SetData([$"{ShipSymbol(ship.Direction)}"]);
            }
        }
    }

    private void MapSun(SystemWaypoint system, int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        float translatedX = 0 - minX;
        float translatedY = 0 - minY;

        int canvasX = singleX
            ? (int)Math.Round(offsetX)
            : (int)Math.Round(offsetX + translatedX * scaleX);

        int canvasY = singleY
            ? (int)Math.Round(offsetY)
            : (int)Math.Round(offsetY + translatedY * scaleY);

        canvasX = Math.Clamp(canvasX, 0, desiredWidth - 1);
        canvasY = Math.Clamp(canvasY, 0, desiredHeight - 1);

        var currentLabel = Binds.GetValueOrDefault("Sun");
        if (currentLabel is null)
        {
            Binds.Add("Sun", Controls.AddLabel($"{StarSymbol(system.SystemType)}", canvasX - 2, canvasY, StarColor(system.SystemType)));
        }
        else
        {
            Binds["Sun"].SetData([$"{StarSymbol(system.SystemType)}"]);
        }
    }

    private void MapWaypoints(int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        foreach (var waypoint in _waypoints.Where(w => string.IsNullOrEmpty(w.Orbits)))
        {
            var hasShips = _ships.Any(s => s.Navigation.WaypointSymbol == waypoint.Symbol && s.Navigation.Status != ShipNavStatus.InTransit);

            float translatedX = waypoint.X - minX;
            float translatedY = waypoint.Y - minY;

            int canvasX = singleX
                ? (int)Math.Round(offsetX)
                : (int)Math.Round(offsetX + translatedX * scaleX);

            int canvasY = singleY
                ? (int)Math.Round(offsetY)
                : (int)Math.Round(offsetY + translatedY * scaleY);

            canvasX = Math.Clamp(canvasX, 0, desiredWidth - 1);
            canvasY = Math.Clamp(canvasY, 0, desiredHeight - 1);

            var currentLabel = _waypointBinds.GetValueOrDefault(waypoint.Symbol);

            if (currentLabel is null)
            {
                _waypointBinds.Add(waypoint.Symbol, Controls.AddLabel($"{WaypointSymbol(waypoint.Type)}", canvasX, canvasY, hasShips ? Color.Cyan : Color.White));
            }
            else
            {
                _waypointBinds[waypoint.Symbol].TextColor = hasShips ? Color.Cyan : Color.White;
                _waypointBinds[waypoint.Symbol].SetData([$"{WaypointSymbol(waypoint.Type)}"]);
            }
        }
    }

    protected override void DrawContent()
    {
        // Content is drawn dynamically in BindData
    }

    private static Color StarColor(SystemType systemType)
    {
        return systemType switch
        {
            SystemType.NeutronStar => Color.Cyan,
            SystemType.RedStar => Color.Red,
            SystemType.OrangeStar => Color.Orange,
            SystemType.BlueStar => Color.Blue,
            SystemType.YoungStar => Color.LightBlue,
            SystemType.WhiteDwarf => Color.White,
            SystemType.BlackHole => Color.DimGray,
            SystemType.HyperGiant => Color.Brown,
            SystemType.Nebula => Color.GreenYellow,
            SystemType.Unstable => Color.Violet,
            _ => Color.Magenta,
        };
    }

    private static string StarSymbol(SystemType systemType)
    {
        return systemType switch
        {
            SystemType.NeutronStar => " <*> ",
            SystemType.RedStar => " (.) ",
            SystemType.OrangeStar => " ( ) ",
            SystemType.BlueStar => " (*) ",
            SystemType.YoungStar => "  o  ",
            SystemType.WhiteDwarf => "  *  ",
            SystemType.BlackHole => " (*) ",
            SystemType.HyperGiant => "(( ))",
            SystemType.Nebula => " ~~~ ",
            SystemType.Unstable => " {!} ",
            _ => " (?) ",
        };
    }

    private static string WaypointSymbol(WaypointType type)
    {
        return type switch
        {
            WaypointType.Planet => $"{(char)9}",
            WaypointType.GasGiant => $"{(char)233}",
            WaypointType.Moon => $"{(char)7}",
            WaypointType.OrbitalStation => "@",
            WaypointType.JumpGate => "*",
            WaypointType.AsteroidField => $"{(char)177}",
            WaypointType.Asteroid => $"{(char)250}",
            WaypointType.EngineeredAsteroid => ";",
            WaypointType.AsteroidBase => "A",
            WaypointType.Nebula => $"{(char)247}",
            WaypointType.DebrisField => "#",
            WaypointType.GravityWell => "v",
            WaypointType.ArtificialGravityWell => "V",
            WaypointType.FuelStation => "&",
            _ => "?",
        };
    }

    private static string ShipSymbol(Core.Enums.Direction direction)
    {
        return direction switch
        {
            Core.Enums.Direction.South => $"{(char)9}",
            Core.Enums.Direction.East => $"{(char)9}",
            Core.Enums.Direction.West => $"{(char)9}",
            Core.Enums.Direction.North => $"{(char)9}",
            _ => "?",
        };
    }
}
