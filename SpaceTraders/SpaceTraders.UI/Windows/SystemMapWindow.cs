using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemMapWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private SystemWaypoint? System { get; set; }
    private Waypoint[] Waypoints { get; set; } = [];

    private readonly SystemService SystemService;
    private readonly WaypointService WaypointService;

    public SystemMapWindow(RootScreen rootScreen, SystemService systemService, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        systemService.Updated += LoadData;
        waypointService.Updated += LoadData;
        SystemService = systemService;
        WaypointService = waypointService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(SystemService.GetSystems().ToArray());
        LoadData(WaypointService.GetWaypoints());
        DrawContent();
    }

    public void LoadData(SystemWaypoint[] data)
    {
        var system = data.FirstOrDefault(d => d.Symbol == Symbol);
        if (System is not null && System == system)
            return;
        Title = $"System {system.Symbol}";
        System = system;
        DrawContent();
    }

    public Task LoadData(ImmutableDictionary<string, ImmutableList<Waypoint>> data)
    {
        var waypoints = data.GetValueOrDefault(Symbol);
        Waypoints = waypoints.ToArray();
        DrawContent();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        Clean();
        if (System is null || !Waypoints.Any())
        {
            Controls.AddLabel($"System loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }

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

        foreach (var waypoint in Waypoints.Where(w => string.IsNullOrEmpty(w.Orbits)))
        {
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

            Controls.AddLabel($"{WaypointSymbol(waypoint.Type)}", canvasX, canvasY, Color.White);
        }
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

            Controls.AddLabel($"{StarSymbol(System.SystemType)}", canvasX - 2, canvasY, StarColor(System.SystemType));
        }

        ResizeAndRedraw();
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

}
