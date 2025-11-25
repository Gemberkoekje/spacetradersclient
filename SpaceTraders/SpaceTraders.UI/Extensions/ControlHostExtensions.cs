using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;

namespace SpaceTraders.UI.Extensions;

public static class ControlHostExtensions
{
    public static void AddLabel(this ControlHost controls, string str, int x, int y, Color? color = null)
    {
        var label = new Label(str)
        {
            Name = str,
        };
        label.TextColor = color;
        label.Position = (x, y);
        controls.Add(label);
    }

    public static void AddButton(this ControlHost controls, string str, int x, int y, EventHandler eventHandler)
    {
        var button = new Button($"{str}")
        {
            Name = $"< {str} >",
        };
        button.Position = new Point(x, y);
        button.Click += eventHandler;
        controls.Add(button);
    }
}
