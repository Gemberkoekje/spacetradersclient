namespace SpaceTraders.Core.Models.ContractModels;

/// <summary>
/// Represents the payment terms for a contract.
/// </summary>
public sealed record ContractPayment
{
    /// <summary>
    /// Gets the payment amount on acceptance.
    /// </summary>
    public decimal OnAccepted { get; init; }

    /// <summary>
    /// Gets the payment amount on fulfillment.
    /// </summary>
    public decimal OnFulfilled { get; init; }
}
