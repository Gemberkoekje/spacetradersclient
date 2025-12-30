using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;
using System.Linq;

namespace SpaceTraders.UI.CustomControls;

/// <summary>
/// A custom list box control with auto-resize capability.
/// </summary>
public sealed class CustomListBox : ListBox, IHaveABottomRightCorner, ICanSetData
{
    private bool ShouldResize { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomListBox"/> class.
    /// </summary>
    /// <param name="width">The width of the list box.</param>
    /// <param name="height">The height of the list box.</param>
    /// <param name="resize">Whether to auto-resize.</param>
    public CustomListBox(int width, int height, bool resize = true)
        : base(width, height)
    {
        ShouldResize = resize;
    }

    /// <summary>
    /// Sets the data for the list box.
    /// </summary>
    /// <param name="text">The data to set.</param>
    public void SetData(object[] text)
    {
        Items.Clear();
        foreach (var item in text)
        {
            Items.Add(item);
        }
        if (ShouldResize)
        {
            Resize(ActualWidth, Height);
        }
        IsDirty = true;
    }

    /// <summary>
    /// Gets the actual width based on content.
    /// </summary>
    public int ActualWidth => Items.Any() ? Items.Max(i => (i.ToString() ?? string.Empty).Length + 2) : 35;

    /// <summary>
    /// Gets the bottom right corner position.
    /// </summary>
    public (int X, int Y) BottomRightCorner => (Position.X + Width, Position.Y + Height);
}

/// <summary>
/// A generic custom list box control with typed items.
/// </summary>
/// <typeparam name="T">The type of items in the list box.</typeparam>
public sealed class CustomListBox<T> : ListBox, IHaveABottomRightCorner, ICanSetData
{
    private bool ShouldResize { get; init; }

    private T[] CustomItems { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomListBox{T}"/> class.
    /// </summary>
    /// <param name="width">The width of the list box.</param>
    /// <param name="height">The height of the list box.</param>
    /// <param name="resize">Whether to auto-resize.</param>
    public CustomListBox(int width, int height, bool resize = true)
        : base(width, height)
    {
        ShouldResize = resize;
    }

    /// <summary>
    /// Sets the custom data items.
    /// </summary>
    /// <param name="items">The items to set.</param>
    public void SetCustomData(T[] items)
    {
        CustomItems = items;
        SetData(items.Cast<object>().ToArray());
    }

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    /// <returns>The selected item.</returns>
    public T GetSelectedItem()
    {
        var selectedIndex = SelectedIndex;
        return CustomItems[selectedIndex];
    }

    /// <summary>
    /// Sets the data for the list box.
    /// </summary>
    /// <param name="text">The data to set.</param>
    public void SetData(object[] text)
    {
        Items.Clear();
        foreach (var item in text)
        {
            Items.Add(item);
        }
        if (ShouldResize)
        {
            Resize(ActualWidth, Height);
        }
        IsDirty = true;
    }

    /// <summary>
    /// Gets the actual width based on content.
    /// </summary>
    public int ActualWidth => Items.Any() ? Items.Max(i => (i.ToString() ?? string.Empty).Length + 2) : 35;

    /// <summary>
    /// Gets the bottom right corner position.
    /// </summary>
    public (int X, int Y) BottomRightCorner => (Position.X + Width, Position.Y + Height);
}
