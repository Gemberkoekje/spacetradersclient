using SadConsole.UI;
using SadConsole.UI.Controls;
using System.Linq;

namespace SpaceTraders.UI.Windows;

public class ClosableWindow : AutoResizableWindow
{
    public ClosableWindow(RootScreen rootScreen, int width, int height)
        : base(rootScreen, width, height)
    {
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
}
