using Qowaiv.Customization;
using Qowaiv.OpenApi;

namespace SpaceTraders.Core.IDs;

/// <summary>
/// Represents a unique identifier for a waypoint (location within a system) in the SpaceTraders universe.
/// </summary>
/// <example>X1-VJ19-H52.</example>
[OpenApiDataType(
    description: "Waypoint Symbol",
    type: "string",
    example: "X1-VJ19-H52")]
[Id<StringIdBehavior, string>]
public readonly partial struct WaypointSymbol { }
