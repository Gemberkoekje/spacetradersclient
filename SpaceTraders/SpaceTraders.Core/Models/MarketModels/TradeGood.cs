using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.MarketModels;

/// <summary>
/// Represents a trade good.
/// </summary>
public sealed record TradeGood
{
    /// <summary>
    /// Gets the trade symbol.
    /// </summary>
    required public TradeSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the trade good name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the trade good description.
    /// </summary>
    required public string Description { get; init; }
}
