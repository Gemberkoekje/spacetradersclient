using SadConsole;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// The main root window of the application.
/// </summary>
public sealed class RootWindow : AutoResizableWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootWindow"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    public RootWindow(RootScreen rootScreen)
        : base(rootScreen, width: Game.Instance.ScreenCellsX, height: Game.Instance.ScreenCellsY)
    {
        Title = "Space Traders";
        DrawContent();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddButton($"Agent", 2, y++, (_, _) => RootScreen.ShowWindow<AgentWindow, Agent>());
        Controls.AddButton($"Ships", 2, y++, (_, _) => RootScreen.ShowWindow<ShipsWindow, ImmutableArray<Ship>>());
        Controls.AddButton($"Contracts", 2, y++, (_, _) => RootScreen.ShowWindow<ContractWindow, Contract>());
        Controls.AddButton($"Known Systems", 2, y++, (_, _) => RootScreen.ShowWindow<SystemsWindow, ImmutableArray<SystemWaypoint>>());
        y++;
        Controls.AddButton($"Debug Glyph Window", 2, y, (_, _) => RootScreen.ShowGlyphWindow());
        ResizeAndRedraw();
    }
}
