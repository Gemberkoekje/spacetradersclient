using Qowaiv.Customization;
using Qowaiv.OpenApi;

namespace SpaceTraders.Core.IDs;

/// <summary>
/// Represents a unique identifier for a sector in the SpaceTraders universe.
/// </summary>
/// <example>X1.</example>
[OpenApiDataType(
    description: "Sector Symbol",
    type: "string",
    example: "X1")]
[Id<StringIdBehavior, string>]
public readonly partial struct SectorSymbol { }
