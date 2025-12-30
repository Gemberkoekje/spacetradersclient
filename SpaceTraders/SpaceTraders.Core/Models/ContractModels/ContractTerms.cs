using System;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ContractModels;

/// <summary>
/// Represents the terms of a contract.
/// </summary>
public sealed record ContractTerms
{
    /// <summary>
    /// Gets the deadline for the contract.
    /// </summary>
    public DateTimeOffset Deadline { get; init; }

    /// <summary>
    /// Gets the payment details for the contract.
    /// </summary>
    required public ContractPayment Payment { get; init; }

    /// <summary>
    /// Gets the goods to be delivered for the contract.
    /// </summary>
    public ImmutableHashSet<ContractDeliverGood> Deliver { get; init; } = [];
}
