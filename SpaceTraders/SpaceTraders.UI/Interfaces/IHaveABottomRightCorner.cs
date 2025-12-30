namespace SpaceTraders.UI.Interfaces;

/// <summary>
/// Interface for controls that have a bottom right corner position.
/// </summary>
public interface IHaveABottomRightCorner
{
    /// <summary>
    /// Gets the bottom right corner position.
    /// </summary>
    public (int X, int Y) BottomRightCorner { get; }
}
