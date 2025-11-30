using SadConsole.UI;
using SadConsole.UI.Controls;
using System.Linq;

namespace SpaceTraders.UI.Windows;

public class AutoResizableWindow : Window
{
    protected RootScreen RootScreen { get; init; }

    protected bool Loaded { get; set; } = false;

    public AutoResizableWindow(RootScreen rootScreen, int width, int height)
        : base(width, height)
    {
        RootScreen = rootScreen;

        CanDrag = true;

        Center();
    }

    protected void ResizeAndRedraw()
    {
        var maxwidth = Controls.Where(c => c.Name != "CloseButton").Max(c => c.Position.X + (c.Name?.Length ?? 0));
        var maxheight = Controls.Where(c => c.Name != "CloseButton").Max(c => c.Position.Y + 1);

        Resize(maxwidth + 2, maxheight + 2, true);
        var closebutton = Controls.SingleOrDefault(c => c.Name == "CloseButton");
        if (closebutton != null)
        {
            closebutton.Position = (Width - 4, 0);
        }
        DrawBorder();
        IsDirty = true;
        if (!Loaded)
        {
            Loaded = true;
            Center();
        }
        if (Position.Y < 0)
        {
            Position = (Position.X, 0);
        }
    }

    protected void Clean()
    {
        foreach (var c in Controls.Where(c => c.Name != "CloseButton").ToList())
        {
            Controls.Remove(c);
        }
    }
}
