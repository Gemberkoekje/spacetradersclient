using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.UI.CustomControls;
using System;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Extensions;

public static class ControlHostExtensions
{
    public static CustomLabel AddLabel(this ControlHost controls, string str, string labelName, int x, int y, Color? color = null)
    {
        var label = new CustomLabel(str)
        {
            Name = labelName,
        };
        label.TextColor = color;
        label.Position = (x, y);
        controls.Add(label);
        return label;
    }

    public static CustomButton AddButton(this ControlHost controls, string str, string buttonName, int x, int y, EventHandler eventHandler, Color? color = null)
    {
        var button = new CustomButton($"{str}")
        {
            Name = buttonName,
        };
        button.Position = new Point(x, y);
        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);
        button.Click += eventHandler;
        controls.Add(button);
        return button;
    }

    public static CustomButton AddAsyncButton(this ControlHost controls, string str, string buttonName, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null, Color? color = null)
    {
        var button = new CustomButton($"{str}")
        {
            Name = buttonName,
            Position = new Point(x, y)
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);

        // Synchronous delegate; internally launches the async work and observes exceptions.
        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
        return button;
    }

    public static CustomLabel AddLabel(this ControlHost controls, string str, int x, int y, Color? color = null)
    {
        var label = new CustomLabel(str)
        {
            Name = str,
        };
        label.TextColor = color;
        label.Position = (x, y);
        controls.Add(label);
        return label;
    }

    public static CustomButton AddButton(this ControlHost controls, string str, int x, int y, EventHandler eventHandler, Color? color = null)
    {
        var button = new CustomButton($"{str}")
        {
            Name = str,
        };
        button.Position = new Point(x, y);
        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);
        button.Click += eventHandler;
        controls.Add(button);
        return button;
    }

    public static CustomButton AddAsyncButton(this ControlHost controls, string str, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null, Color? color = null)
    {
        var button = new CustomButton($"{str}")
        {
            Name = str,
            Position = new Point(x, y)
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);

        // Synchronous delegate; internally launches the async work and observes exceptions.
        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
        return button;
    }

    public static CustomListBox AddListbox(this ControlHost controls, string name, int x, int y, int width, int height, Color? color = null)
    {
        var listbox = new CustomListBox(width, height)
        {
            Name = name,
            Position = new Point(x, y),
            DrawBorder = true,
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        colors.Lines.SetColor(Color.DarkSlateGray);
        listbox.SetThemeColors(colors);

        controls.Add(listbox);
        return listbox;
    }
}
