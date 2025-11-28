using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class StarMapWindow : ClosableWindow
{
    private List<SystemWaypoint> Systems { get; set; } = [];

    public StarMapWindow(RootScreen rootScreen, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        systemService.Updated += LoadData;
        LoadData(systemService.GetSystems().ToArray());
    }

    public void LoadData(SystemWaypoint[] data)
    {
        Title = $"Starmap";
        Systems.AddRange(data);
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Systems.Count == 0)
        {
            Controls.AddLabel($"System loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }

        // Compute extents.
        int minX = Systems.Min(w => w.X);
        int minY = Systems.Min(w => w.Y);
        int maxX = Systems.Max(w => w.X);
        int maxY = Systems.Max(w => w.Y);

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

        foreach (var waypoint in Systems)
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

            Controls.AddLabel($"{(char)250}", canvasX, canvasY, StarColor(waypoint.SystemType));
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
}
