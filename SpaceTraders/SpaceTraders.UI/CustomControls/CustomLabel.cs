using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.CustomControls;

/// <summary>
/// A custom label control with data binding support.
/// </summary>
public sealed class CustomLabel : Label, IHaveABottomRightCorner, ICanSetData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLabel"/> class with a specified width.
    /// </summary>
    /// <param name="width">The width of the label.</param>
    public CustomLabel(int width)
        : base(width)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLabel"/> class with specified text.
    /// </summary>
    /// <param name="displayText">The display text of the label.</param>
    public CustomLabel(string displayText)
        : base(displayText)
    {
    }

    /// <summary>
    /// Sets the data for the label.
    /// </summary>
    /// <param name="text">The data to set.</param>
    public void SetData(object[] text)
    {
        DisplayText = text[0]?.ToString() ?? string.Empty;
        Resize(ActualWidth, Height);
        IsDirty = true;
    }

    /// <summary>
    /// Gets the actual width of the label.
    /// </summary>
    public int ActualWidth => DisplayText.Length;

    /// <summary>
    /// Gets the bottom right corner position.
    /// </summary>
    public (int X, int Y) BottomRightCorner => (Position.X + ActualWidth, Position.Y + Height);
}
