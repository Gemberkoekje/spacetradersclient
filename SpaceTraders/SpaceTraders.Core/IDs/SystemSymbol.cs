using Qowaiv.Customization;
using Qowaiv.OpenApi;

namespace SpaceTraders.Core.IDs;

/// <summary>
/// Represents a unique identifier for a star system in the SpaceTraders universe.
/// </summary>
/// <example>X1-VJ19.</example>
[OpenApiDataType(
    description: "System Symbol",
    type: "string",
    example: "X1-VJ19")]
[Id<StringIdBehavior, string>]
public readonly partial struct SystemSymbol { }
