using SadConsole.UI;
using SadConsole.UI.Controls;
using System.Linq;

namespace SpaceTraders.UI.Windows;

public class ClosableWindow : Window
{
    protected RootScreen RootScreen { get; init; }

    public ClosableWindow(RootScreen rootScreen, int width, int height)
        : base(width, height)
    {
        RootScreen = rootScreen;

        CanDrag = true;

        var closeButton = new Button(3)
        {
            Name = "CloseButton",
            Text = "X",
            Position = (Width - 4, 0),
        };
        closeButton.Click += (_, _) => RootScreen.HideAndDestroyWindow(this);
        Controls.Add(closeButton);
        Center();
    }

    protected void ResizeAndRedraw()
    {
        var maxwidth = Controls.Where(c => c.Name != "CloseButton").Max(c => c.Position.X + (c.Name?.Length ?? 0));
        var maxheight = Controls.Where(c => c.Name != "CloseButton").Max(c => c.Position.Y + 1);

        Resize(maxwidth + 2, maxheight + 2, true);
        Controls.Single(c => c.Name == "CloseButton").Position = (Width - 4, 0);
        DrawBorder();
        IsDirty = true;
        Center();
        if (Position.Y < 0)
        {
            Position = (Position.X, 0);
        }
    }
}
