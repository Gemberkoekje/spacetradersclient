using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.CustomControls;

/// <summary>
/// A custom button control with data binding support.
/// </summary>
public sealed class CustomButton : Button, IHaveABottomRightCorner, ICanSetData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomButton"/> class with a specified width.
    /// </summary>
    /// <param name="width">The width of the button.</param>
    public CustomButton(int width)
        : base(width)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomButton"/> class with specified text.
    /// </summary>
    /// <param name="text">The text of the button.</param>
    public CustomButton(string text)
        : base(text)
    {
    }

    /// <summary>
    /// Sets the data for the button.
    /// </summary>
    /// <param name="text">The data to set.</param>
    public void SetData(object[] text)
    {
        Text = text[0]?.ToString() ?? string.Empty;
        IsDirty = true;
    }

    /// <summary>
    /// Gets the actual width of the button.
    /// </summary>
    public int ActualWidth => Text.Length + 4;

    /// <summary>
    /// Gets the bottom right corner position.
    /// </summary>
    public (int X, int Y) BottomRightCorner => (Position.X + ActualWidth, Position.Y + Height);
}
