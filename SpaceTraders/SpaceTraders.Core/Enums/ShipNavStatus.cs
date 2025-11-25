namespace SpaceTraders.Core.Enums;

/// <summary>
/// Represents the current positional state of a ship relative to a waypoint or facility.
/// </summary>
/// <remarks>
/// Status transitions typically occur via navigation commands (entering orbit, docking) or automatically when
/// a travel route completes (arriving and switching from <see cref="InTransit"/> to <see cref="InOrbit"/>).
/// </remarks>
public enum ShipNavStatus
{
    /// <summary>
    /// The ship is currently traveling along its plotted route and has not yet arrived at the destination waypoint.
    /// </summary>
    InTransit,

    /// <summary>
    /// The ship has arrived and is in a stable orbit around the destination waypoint, able to perform most actions.
    /// </summary>
    InOrbit,

    /// <summary>
    /// The ship is docked at a facility (e.g. station, shipyard or outpost) enabling cargo transfer and other services.
    /// </summary>
    Docked,
}
