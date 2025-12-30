using SadConsole;
using SpaceTraders.UI.Extensions;

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
        Controls.AddButton($"Agent", 2, y++, (_, _) => RootScreen.ShowWindow<AgentWindow>([]));
        Controls.AddButton($"Ships", 2, y++, (_, _) => RootScreen.ShowWindow<ShipsWindow>([]));
        Controls.AddButton($"Contracts", 2, y++, (_, _) => RootScreen.ShowWindow<ContractWindow>([]));
        Controls.AddButton($"Known Systems", 2, y++, (_, _) => RootScreen.ShowWindow<SystemsWindow>([]));
        y++;
        Controls.AddButton($"Debug Glyph Window", 2, y, (_, _) => RootScreen.ShowGlyphWindow());
        ResizeAndRedraw();
    }
}
