namespace SpaceTraders.Core.Enums;

/// <summary>
/// Defines the functional specialization of a ship within the SpaceTraders universe.
/// </summary>
/// <remarks>
/// Roles influence available modules, typical missions, and optimal assignments.
/// </remarks>
public enum ShipRole
{
    /// <summary>
    /// Focused on manufacturing goods or components from raw materials.
    /// </summary>
    Fabricator,

    /// <summary>
    /// Collects raw resources (minerals, gases, organics) from waypoints.
    /// </summary>
    Harvester,

    /// <summary>
    /// Optimized for cargo capacity and transport logistics.
    /// </summary>
    Hauler,

    /// <summary>
    /// Pursues and engages other ships; combat interception.
    /// </summary>
    Interceptor,

    /// <summary>
    /// Performs deep resource extraction from asteroids or planetary surfaces.
    /// </summary>
    Excavator,

    /// <summary>
    /// Moves crew, passengers, or cargo between waypoints efficiently.
    /// </summary>
    Transport,

    /// <summary>
    /// Provides hull and system repair services to other ships or self.
    /// </summary>
    Repair,

    /// <summary>
    /// Conducts planetary and space surveys to discover resources and traits.
    /// </summary>
    Surveyor,

    /// <summary>
    /// Serves as the fleet coordination or leadership vessel.
    /// </summary>
    Command,

    /// <summary>
    /// Carries smaller vessels or fighters; mobile deployment platform.
    /// </summary>
    Carrier,

    /// <summary>
    /// Performs security patrols of sectors or waypoints.
    /// </summary>
    Patrol,

    /// <summary>
    /// Stationary or orbital support platform providing passive services or monitoring.
    /// </summary>
    Satellite,

    /// <summary>
    /// Explores unknown sectors, scanning and charting new waypoints.
    /// </summary>
    Explorer,

    /// <summary>
    /// Processes raw materials into refined goods.
    /// </summary>
    Refinery,
}
