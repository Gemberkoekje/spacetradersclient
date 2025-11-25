namespace SpaceTraders.Core.Enums;

/// <summary>
/// Indicates the current propulsion / movement profile of a ship while navigating between waypoints.
/// </summary>
/// <remarks>
/// Flight mode affects fuel consumption, travel time, and detectability. Modes typically can be changed via the navigation API.
/// </remarks>
public enum ShipNavFlightMode
{
    /// <summary>
    /// Minimal thrust, drifting slowly. Lowest fuel usage; longest travel time.
    /// </summary>
    Drift,

    /// <summary>
    /// Reduced emissions profile. Harder to detect; slower and slightly less fuel efficient than cruise.
    /// </summary>
    Stealth,

    /// <summary>
    /// Standard travel speed. Balanced fuel usage and travel time.
    /// </summary>
    Cruise,

    /// <summary>
    /// Maximum thrust. Fastest travel; highest fuel consumption and detectability.
    /// </summary>
    Burn,
}
