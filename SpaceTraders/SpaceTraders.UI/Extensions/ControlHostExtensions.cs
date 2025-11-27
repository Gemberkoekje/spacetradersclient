using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Extensions;

public static class ControlHostExtensions
{
    public static void AddLabel(this ControlHost controls, string str, int x, int y, Color? color = null)
    {
        var label = new SadConsole.UI.Controls.Label(str)
        {
            Name = str,
        };
        label.TextColor = color;
        label.Position = (x, y);
        controls.Add(label);
    }

    public static void AddButton(this ControlHost controls, string str, int x, int y, EventHandler eventHandler, Color? color = null)
    {
        var button = new Button($"{str}")
        {
            Name = $"< {str} >",
        };
        button.Position = new Point(x, y);
        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);
        button.Click += eventHandler;
        controls.Add(button);
    }

    public static void AddAsyncButton(this ControlHost controls, string str, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null, Color? color = null)
    {
        var button = new Button($"{str}")
        {
            Name = $"< {str} >",
            Position = new Point(x, y)
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);

        // Synchronous delegate; internally launches the async work and observes exceptions.
        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
    }
}
