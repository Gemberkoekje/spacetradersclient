using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.UI.Extensions;
using System;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal class SystemMapWindow : ClosableWindow
{
    private SystemWaypoint System { get; init; }

    public SystemMapWindow(SystemWaypoint waypoint, RootScreen rootScreen)
        : base(rootScreen, 45, 30)
    {
        Title = $"System {waypoint.Symbol}";
        System = waypoint;
        DrawContent();
    }

    private void DrawContent()
    {
        if (System is null)
            return;

        // Compute extents.
        int minX = System.Waypoints.Min(w => w.X);
        int minY = System.Waypoints.Min(w => w.Y);
        int maxX = System.Waypoints.Max(w => w.X);
        int maxY = System.Waypoints.Max(w => w.Y);

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

        foreach (var waypoint in System.Waypoints.Where(w => string.IsNullOrEmpty(w.Orbits)))
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
