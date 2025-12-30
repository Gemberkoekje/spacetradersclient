using SpaceTraders.Core.Enums;

namespace SpaceTraders.Core.Models.MarketModels;

/// <summary>
/// Represents a trade good at a market with pricing information.
/// </summary>
public sealed record MarketTradeGood
{
    /// <summary>
    /// Gets the trade symbol.
    /// </summary>
    required public TradeSymbol Symbol { get; init; }

    /// <summary>
    /// Gets the trade good type.
    /// </summary>
    required public MarketTradeGoodType Type { get; init; }

    /// <summary>
    /// Gets the trade volume.
    /// </summary>
    required public int TradeVolume { get; init; }

    /// <summary>
    /// Gets the supply level.
    /// </summary>
    required public SupplyLevel Supply { get; init; }

    /// <summary>
    /// Gets the activity level.
    /// </summary>
    required public ActivityLevel Activity { get; init; }

    /// <summary>
    /// Gets the purchase price.
    /// </summary>
    required public int PurchasePrice { get; init; }

    /// <summary>
    /// Gets the sell price.
    /// </summary>
    required public int SellPrice { get; init; }
}
