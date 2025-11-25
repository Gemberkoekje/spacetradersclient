namespace SpaceTraders.Core.Enums;

/// <summary>
/// Enumerates the distinct kinds of navigable waypoints or spatial objects in the SpaceTraders universe.
/// </summary>
/// <remarks>
/// Waypoint type influences available actions, resources, and navigation behavior.
/// </remarks>
public enum WaypointType
{
    /// <summary>
    /// A terrestrial or rocky planet.
    /// </summary>
    Planet,

    /// <summary>
    /// A large gas giant, often suitable for fuel harvesting.
    /// </summary>
    GasGiant,

    /// <summary>
    /// A natural satellite orbiting a planet.
    /// </summary>
    Moon,

    /// <summary>
    /// A man-made station in stable orbit providing services and docking.
    /// </summary>
    OrbitalStation,

    /// <summary>
    /// A structure enabling hyperspace jumps between systems.
    /// </summary>
    JumpGate,

    /// <summary>
    /// A region dense with asteroids; resource extraction hotspot.
    /// </summary>
    AsteroidField,

    /// <summary>
    /// A single asteroid object.
    /// </summary>
    Asteroid,

    /// <summary>
    /// An asteroid modified or constructed for specific industrial purpose.
    /// </summary>
    EngineeredAsteroid,

    /// <summary>
    /// A base facility built on or within an asteroid.
    /// </summary>
    AsteroidBase,

    /// <summary>
    /// A nebula cloud; may affect sensor performance and contain rare gases.
    /// </summary>
    Nebula,

    /// <summary>
    /// A field of debris from wrecks or destroyed structures.
    /// </summary>
    DebrisField,

    /// <summary>
    /// A natural gravity anomaly affecting navigation plots.
    /// </summary>
    GravityWell,

    /// <summary>
    /// An artificial gravity distortion created by technology.
    /// </summary>
    ArtificialGravityWell,

    /// <summary>
    /// A refueling outpost or station for replenishing ship fuel reserves.
    /// </summary>
    FuelStation,
}
