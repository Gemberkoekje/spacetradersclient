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

    public static void AddAsyncButton(this ControlHost controls, string str, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null)
    {
        var button = new Button($"{str}")
        {
            Name = $"< {str} >",
            Position = new Point(x, y)
        };

        // Synchronous delegate; internally launches the async work and observes exceptions.
        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
    }
}
