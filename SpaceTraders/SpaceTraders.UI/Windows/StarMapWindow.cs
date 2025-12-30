using SadRogue.Primitives;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class StarMapWindow : DataBoundWindowNoSymbols<ImmutableArray<SystemWaypoint>>
{
    private readonly SystemService _systemService;
    private readonly List<SystemWaypoint> _systems = [];

    public StarMapWindow(RootScreen rootScreen, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        _systemService = systemService;
        Title = "Starmap";

        SubscribeToEvent<ImmutableArray<SystemWaypoint>>(
            handler => systemService.Updated += handler,
            handler => systemService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize(refreshImmediately: true);
    }

    protected override ImmutableArray<SystemWaypoint> FetchData() =>
        _systemService.GetSystems();

    protected override void BindData(ImmutableArray<SystemWaypoint> data)
    {
        _systems.Clear();
        _systems.AddRange(data);
        DrawStarMap();
    }

    private void DrawStarMap()
    {
        Clean();

        if (_systems.Count == 0)
        {
            Controls.AddLabel($"System loading...", 2, 2);
            return;
        }

        int minX = _systems.Min(w => w.X);
        int minY = _systems.Min(w => w.Y);
        int maxX = _systems.Max(w => w.X);
        int maxY = _systems.Max(w => w.Y);

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

        foreach (var waypoint in _systems)
        {
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

            Controls.AddLabel($"{(char)250}", canvasX, canvasY, StarColor(waypoint.SystemType));
        }
    }

    protected override void DrawContent()
    {
        // Content is drawn dynamically in BindData via DrawStarMap
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
}
