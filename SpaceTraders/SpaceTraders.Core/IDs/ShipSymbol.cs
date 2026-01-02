using Qowaiv.Customization;
using Qowaiv.OpenApi;

namespace SpaceTraders.Core.IDs;

/// <summary>
/// Represents a unique identifier for a ship in the SpaceTraders universe.
/// </summary>
/// <example>SP3CT3R-1.</example>
[OpenApiDataType(
    description: "Ship Symbol",
    type: "string",
    example: "SP3CT3R-1")]
[Id<StringIdBehavior, string>]
public readonly partial struct ShipSymbol { }
