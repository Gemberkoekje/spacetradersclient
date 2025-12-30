using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SpaceTraders.UI.CustomControls;
using System;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Extensions;

/// <summary>
/// Extension methods for the ControlHost class.
/// </summary>
public static class ControlHostExtensions
{
    /// <summary>
    /// Adds a label with a specified name to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The label text.</param>
    /// <param name="labelName">The label name.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created label.</returns>
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

    /// <summary>
    /// Adds a label to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The label text.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created label.</returns>
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

    /// <summary>
    /// Adds a button with a specified name to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The button text.</param>
    /// <param name="buttonName">The button name.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="eventHandler">The click event handler.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created button.</returns>
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

    /// <summary>
    /// Adds a button to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The button text.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="eventHandler">The click event handler.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created button.</returns>
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

    /// <summary>
    /// Adds an async button with a specified name to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The button text.</param>
    /// <param name="buttonName">The button name.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="onClickAsync">The async click handler.</param>
    /// <param name="onError">The optional error handler.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created button.</returns>
#pragma warning disable S107 // Methods should not have too many parameters
    public static CustomButton AddAsyncButton(this ControlHost controls, string str, string buttonName, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null, Color? color = null)
#pragma warning restore S107 // Methods should not have too many parameters
    {
        var button = new CustomButton($"{str}")
        {
            Name = buttonName,
            Position = new Point(x, y),
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);

        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
        return button;
    }

    /// <summary>
    /// Adds an async button to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="str">The button text.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="onClickAsync">The async click handler.</param>
    /// <param name="onError">The optional error handler.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created button.</returns>
    public static CustomButton AddAsyncButton(this ControlHost controls, string str, int x, int y, Func<Task> onClickAsync, Action<Exception>? onError = null, Color? color = null)
    {
        var button = new CustomButton($"{str}")
        {
            Name = str,
            Position = new Point(x, y),
        };

        var colors = Colors.CreateAnsi();
        colors.Appearance_ControlNormal.Foreground = color ?? colors.Appearance_ControlNormal.Foreground;
        button.SetThemeColors(colors);

        button.Click += (_, _) => onClickAsync().SafeFireAndForget(onError);

        controls.Add(button);
        return button;
    }

    /// <summary>
    /// Adds a listbox to the control host.
    /// </summary>
    /// <param name="controls">The control host.</param>
    /// <param name="name">The listbox name.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="resize">Whether to auto-resize.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created listbox.</returns>
#pragma warning disable S107 // Methods should not have too many parameters
    public static CustomListBox AddListbox(this ControlHost controls, string name, int x, int y, int width, int height, bool resize = true, Color? color = null)
#pragma warning restore S107 // Methods should not have too many parameters
    {
        var listbox = new CustomListBox(width, height, resize)
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

    /// <summary>
    /// Adds a generic listbox to the control host.
    /// </summary>
    /// <typeparam name="T">The type of items in the listbox.</typeparam>
    /// <param name="controls">The control host.</param>
    /// <param name="name">The listbox name.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="resize">Whether to auto-resize.</param>
    /// <param name="color">The optional color.</param>
    /// <returns>The created listbox.</returns>
#pragma warning disable S107 // Methods should not have too many parameters
    public static CustomListBox<T> AddListbox<T>(this ControlHost controls, string name, int x, int y, int width, int height, bool resize = true, Color? color = null)
#pragma warning restore S107 // Methods should not have too many parameters
    {
        var listbox = new CustomListBox<T>(width, height, resize)
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
