using Qowaiv.Customization;
using Qowaiv.OpenApi;

namespace SpaceTraders.Core.IDs;

/// <summary>
/// Represents a unique identifier for an agent in the SpaceTraders universe.
/// </summary>
/// <example>SP3CT3R.</example>
[OpenApiDataType(
    description: "Agent Symbol",
    type: "string",
    example: "SP3CT3R")]
[Id<StringIdBehavior, string>]
public readonly partial struct AgentSymbol { }
