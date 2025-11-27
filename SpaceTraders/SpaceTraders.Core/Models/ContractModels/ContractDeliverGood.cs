namespace SpaceTraders.Core.Models.ContractModels;

public sealed record ContractDeliverGood
{
    public string TradeSymbol { get; init; }
    public string DestinationSymbol { get; init; }
    public int UnitsRequired { get; init; }
    public int UnitsFulfilled { get; init; }
}
