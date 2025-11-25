using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.UI.Extensions;
using System.Linq;

namespace SpaceTraders.UI.Windows;

public sealed class RootWindow : Window
{
    private RootScreen RootScreen;

    public RootWindow(RootScreen rootScreen)
        : base(width: Game.Instance.ScreenCellsX, height: Game.Instance.ScreenCellsY)
    {
        Title = "Space Traders";
        CanDrag = false;
        DrawContent();
        RootScreen = rootScreen;
    }

    public void ResizeAndRedraw(int width, int height, bool clear = false)
    {
        Resize(width, height, clear);
        DrawBorder();          // Recalculate title area & border with new size.
        IsDirty = true;        // Ensure redraw.
        Controls.First(c => c.Name == "< Debug Glyph Window >").Position = new Point(2, height - 2);
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddButton($"Agent", 2, y++, (_, _) => RootScreen.ShowAgentWindow());
        Controls.AddButton($"Ships", 2, y++, (_, _) => RootScreen.ShowShipsWindow());
        Controls.AddButton($"Debug Glyph Window", 2, Game.Instance.ScreenCellsY - 2, (_, _) => RootScreen.ShowGlyphWindow());
    }
}