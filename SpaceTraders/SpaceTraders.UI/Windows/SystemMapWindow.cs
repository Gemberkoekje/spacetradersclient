using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemMapWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private SystemWaypoint? System { get; set; }
    private Waypoint[] Waypoints { get; set; } = [];
    private Ship[] Ships { get; set; } = [];

    private Dictionary<string, CustomLabel> WaypointBinds { get; set; } = new Dictionary<string, CustomLabel>();

    private readonly SystemService SystemService;
    private readonly WaypointService WaypointService;
    private readonly ShipService ShipService;

    public SystemMapWindow(RootScreen rootScreen, SystemService systemService, WaypointService waypointService, ShipService shipService)
        : base(rootScreen, 45, 30)
    {
        systemService.Updated += LoadData;
        waypointService.Updated += LoadData;
        shipService.Updated += LoadData;
        SystemService = systemService;
        WaypointService = waypointService;
        ShipService = shipService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(WaypointService.GetWaypoints());
        LoadData(SystemService.GetSystems().ToArray());
        LoadData(ShipService.GetShips().ToArray());
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var relevantData = data.Where(d => d.Navigation.SystemSymbol == Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;
        Ships = relevantData;

        if (Waypoints.Length == 0)
            return Task.CompletedTask;
        // Compute extents.
        int minX = Waypoints.Min(w => w.X);
        int minY = Waypoints.Min(w => w.Y);
        int maxX = Waypoints.Max(w => w.X);
        int maxY = Waypoints.Max(w => w.Y);

        int desiredWidth = (int)(50.0 * ((float)SadConsole.Game.Instance.DefaultFont.GlyphHeight / (float)SadConsole.Game.Instance.DefaultFont.GlyphWidth));
        int desiredHeight = 50;

        int rangeX = maxX - minX;
        int rangeY = maxY - minY;

        // Avoid divide by zero; if all X (or Y) are identical, place them centered.
        bool singleX = rangeX == 0;
        bool singleY = rangeY == 0;

        // Pre-calc scaling factors (float to preserve distribution, cast later).
        float scaleX = singleX ? 0f : (desiredWidth - 1f) / rangeX;
        float scaleY = singleY ? 0f : (desiredHeight - 1f) / rangeY;

        // Optional: keep aspect ratio by using the smaller scale and centering.
        float offsetX = 2f;
        float offsetY = 2f;

        // Center if degenerate.
        if (singleX) offsetX = desiredWidth / 2f;
        if (singleY) offsetY = desiredHeight / 2f;

        MapShips(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        MapWaypoints(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);

        ResizeAndRedraw();
        return Task.CompletedTask;

    }

    private void MapShips(int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        foreach (var ship in Ships.Where(s => s.Navigation.Status == ShipNavStatus.InTransit))
        {
            float translatedX = ship.Position.Value.X - minX;
            float translatedY = ship.Position.Value.Y - minY;

            int canvasX = singleX
                ? (int)Math.Round(offsetX)
                : (int)Math.Round(offsetX + translatedX * scaleX);

            int canvasY = singleY
                ? (int)Math.Round(offsetY)
                : (int)Math.Round(offsetY + translatedY * scaleY);

            // Clamp just in case of rounding overflow.
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

    public void LoadData(SystemWaypoint[] data)
    {
        if (Surface == null)
            return;
        var system = data.FirstOrDefault(d => d.Symbol == Symbol);
        if (System is not null && System == system)
            return;
        if (system is null)
        {
            return;
        }
        Title = $"System {system.Symbol}";
        System = system;
        if (Waypoints.Length == 0)
            return;
        // Compute extents.
        int minX = Waypoints.Min(w => w.X);
        int minY = Waypoints.Min(w => w.Y);
        int maxX = Waypoints.Max(w => w.X);
        int maxY = Waypoints.Max(w => w.Y);

        int desiredWidth = (int)(50.0 * ((float)SadConsole.Game.Instance.DefaultFont.GlyphHeight / (float)SadConsole.Game.Instance.DefaultFont.GlyphWidth));
        int desiredHeight = 50;

        int rangeX = maxX - minX;
        int rangeY = maxY - minY;

        // Avoid divide by zero; if all X (or Y) are identical, place them centered.
        bool singleX = rangeX == 0;
        bool singleY = rangeY == 0;

        // Pre-calc scaling factors (float to preserve distribution, cast later).
        float scaleX = singleX ? 0f : (desiredWidth - 1f) / rangeX;
        float scaleY = singleY ? 0f : (desiredHeight - 1f) / rangeY;

        // Optional: keep aspect ratio by using the smaller scale and centering.
        float offsetX = 2f;
        float offsetY = 2f;

        // Center if degenerate.
        if (singleX) offsetX = desiredWidth / 2f;
        if (singleY) offsetY = desiredHeight / 2f;
        MapSun(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        ResizeAndRedraw();
    }

    private void MapSun(int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        float translatedX = 0 - minX;
        float translatedY = 0 - minY;

        int canvasX = singleX
            ? (int)Math.Round(offsetX)
            : (int)Math.Round(offsetX + translatedX * scaleX);

        int canvasY = singleY
            ? (int)Math.Round(offsetY)
            : (int)Math.Round(offsetY + translatedY * scaleY);

        // Clamp just in case of rounding overflow.
        canvasX = Math.Clamp(canvasX, 0, desiredWidth - 1);
        canvasY = Math.Clamp(canvasY, 0, desiredHeight - 1);

        var currentLabel = Binds.GetValueOrDefault("Sun");
        if (currentLabel is null)
        {
            Binds.Add("Sun", Controls.AddLabel($"{StarSymbol(System.SystemType)}", canvasX - 2, canvasY, StarColor(System.SystemType)));
        }
        else
        {
            Binds["Sun"].SetData([$"{StarSymbol(System.SystemType)}"]);
        }
    }

    public Task LoadData(ImmutableDictionary<string, ImmutableList<Waypoint>> data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var waypoints = data.GetValueOrDefault(Symbol);
        Waypoints = waypoints.ToArray();

        // Compute extents.
        int minX = Waypoints.Min(w => w.X);
        int minY = Waypoints.Min(w => w.Y);
        int maxX = Waypoints.Max(w => w.X);
        int maxY = Waypoints.Max(w => w.Y);

        int desiredWidth = (int)(50.0 * ((float)SadConsole.Game.Instance.DefaultFont.GlyphHeight / (float)SadConsole.Game.Instance.DefaultFont.GlyphWidth));
        int desiredHeight = 50;

        int rangeX = maxX - minX;
        int rangeY = maxY - minY;

        // Avoid divide by zero; if all X (or Y) are identical, place them centered.
        bool singleX = rangeX == 0;
        bool singleY = rangeY == 0;

        // Pre-calc scaling factors (float to preserve distribution, cast later).
        float scaleX = singleX ? 0f : (desiredWidth - 1f) / rangeX;
        float scaleY = singleY ? 0f : (desiredHeight - 1f) / rangeY;

        // Optional: keep aspect ratio by using the smaller scale and centering.
        float offsetX = 2f;
        float offsetY = 2f;

        // Center if degenerate.
        if (singleX) offsetX = desiredWidth / 2f;
        if (singleY) offsetY = desiredHeight / 2f;
        MapWaypoints(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        if (System is not null)
        {
            MapSun(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        }
        MapShips(minX, minY, desiredWidth, desiredHeight, singleX, singleY, scaleX, scaleY, offsetX, offsetY);
        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    private void MapWaypoints(int minX, int minY, int desiredWidth, int desiredHeight, bool singleX, bool singleY, float scaleX, float scaleY, float offsetX, float offsetY)
    {
        foreach (var waypoint in Waypoints.Where(w => string.IsNullOrEmpty(w.Orbits)))
        {
            var hasShips = Ships.Any(s => s.Navigation.WaypointSymbol == waypoint.Symbol && s.Navigation.Status != ShipNavStatus.InTransit);

            float translatedX = waypoint.X - minX;
            float translatedY = waypoint.Y - minY;

            int canvasX = singleX
                ? (int)Math.Round(offsetX)
                : (int)Math.Round(offsetX + translatedX * scaleX);

            int canvasY = singleY
                ? (int)Math.Round(offsetY)
                : (int)Math.Round(offsetY + translatedY * scaleY);

            // Clamp just in case of rounding overflow.
            canvasX = Math.Clamp(canvasX, 0, desiredWidth - 1);
            canvasY = Math.Clamp(canvasY, 0, desiredHeight - 1);

            var currentLabel = WaypointBinds.GetValueOrDefault(waypoint.Symbol);

            if (currentLabel is null)
            {
                WaypointBinds.Add(waypoint.Symbol, Controls.AddLabel($"{WaypointSymbol(waypoint.Type)}", canvasX, canvasY, hasShips ? Color.Cyan : Color.White));
            }
            else
            {
                WaypointBinds[waypoint.Symbol].TextColor = hasShips ? Color.Cyan : Color.White;
                WaypointBinds[waypoint.Symbol].SetData([$"{WaypointSymbol(waypoint.Type)}"]);
            }
        }
    }

    private void DrawContent()
    {

    }

    private Color StarColor(SystemType systemType)
    {
        switch(systemType)
        {
            case SystemType.NeutronStar:
                return Color.Cyan;
            case SystemType.RedStar:
                return Color.Red;
            case SystemType.OrangeStar:
                return Color.Orange;
            case SystemType.BlueStar:
                return Color.Blue;
            case SystemType.YoungStar:
                return Color.LightBlue;
            case SystemType.WhiteDwarf:
                return Color.White;
            case SystemType.BlackHole:
                return Color.DimGray;
            case SystemType.HyperGiant:
                return Color.Brown;
            case SystemType.Nebula:
                return Color.GreenYellow;
            case SystemType.Unstable:
                return Color.Violet;
            default:
                return Color.Magenta;
        }
    }

    private string StarSymbol(SystemType systemType)
    {
        switch (systemType)
        {
            case SystemType.NeutronStar:
                return " <*> ";
            case SystemType.RedStar:
                return " (.) ";
            case SystemType.OrangeStar:
                return " ( ) ";
            case SystemType.BlueStar:
                return " (*) ";
            case SystemType.YoungStar:
                return "  o  ";
            case SystemType.WhiteDwarf:
                return "  *  ";
            case SystemType.BlackHole:
                return " (*) ";
            case SystemType.HyperGiant:
                return "(( ))";
            case SystemType.Nebula:
                return " ~~~ ";
            case SystemType.Unstable:
                return " {!} ";
            default:
                return " (?) ";
        }
    }

    private string WaypointSymbol(WaypointType type)
    {
        switch (type)
        {
            case WaypointType.Planet:
                return $"{(char)9}";
            case WaypointType.GasGiant:
                return $"{(char)233}";
            case WaypointType.Moon:
                return $"{(char)7}";
            case WaypointType.OrbitalStation:
                return "@";
            case WaypointType.JumpGate:
                return "*";
            case WaypointType.AsteroidField:
                return $"{(char)177}";
            case WaypointType.Asteroid:
                return $"{(char)250}";
            case WaypointType.EngineeredAsteroid:
                return ";";
            case WaypointType.AsteroidBase:
                return "A";
            case WaypointType.Nebula:
                return $"{(char)247}";
            case WaypointType.DebrisField:
                return "#";
            case WaypointType.GravityWell:
                return "v";
            case WaypointType.ArtificialGravityWell:
                return "V";
            case WaypointType.FuelStation:
                return "&";
            default:
                return "?";
        }
    }

    private string ShipSymbol(Core.Enums.Direction direction)
    {
        switch (direction)
        {
            case Core.Enums.Direction.South:
                return $"{(char)9}";
            case Core.Enums.Direction.East:
                return $"{(char)9}";
            case Core.Enums.Direction.West:
                return $"{(char)9}";
            case Core.Enums.Direction.North:
                return $"{(char)9}";
            default:
                return "?";
        }
    }

}
