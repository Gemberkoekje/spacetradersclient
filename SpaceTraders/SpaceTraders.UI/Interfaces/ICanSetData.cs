namespace SpaceTraders.UI.Interfaces;

/// <summary>
/// Interface for controls that can have data set.
/// </summary>
public interface ICanSetData
{
    /// <summary>
    /// Sets the data for the control.
    /// </summary>
    /// <param name="text">The data to set.</param>
    public void SetData(object[] text);
}
