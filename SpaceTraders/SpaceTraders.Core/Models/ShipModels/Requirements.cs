namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Represents the requirements for a ship component.
/// </summary>
public sealed record Requirements
{
    /// <summary>
    /// Gets the power requirement.
    /// </summary>
    required public int Power { get; init; }

    /// <summary>
    /// Gets the crew requirement.
    /// </summary>
    required public int Crew { get; init; }

    /// <summary>
    /// Gets the slots requirement.
    /// </summary>
    required public int Slots { get; init; }
}
