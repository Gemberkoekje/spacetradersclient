namespace SpaceTraders.Core.Models.ContractModels;

public sealed record ContractPayment
{
    public decimal OnAccepted { get; init; }
    public decimal OnFulfilled { get; init; }
}
