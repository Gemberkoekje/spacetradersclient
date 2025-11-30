using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.UI.Extensions;
using System.Linq;

namespace SpaceTraders.UI.Windows;

public sealed class RootWindow : AutoResizableWindow
{
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
        Controls.AddButton($"Debug Glyph Window", 2, y++, (_, _) => RootScreen.ShowGlyphWindow());
        ResizeAndRedraw();
    }
}