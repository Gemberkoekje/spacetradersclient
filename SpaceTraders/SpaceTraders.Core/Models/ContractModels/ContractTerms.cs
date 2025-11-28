using System;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ContractModels;

public sealed record ContractTerms
{
    public DateTimeOffset Deadline { get; init; }
    public ContractPayment Payment { get; init; }
    public ImmutableHashSet<ContractDeliverGood> Deliver { get; init; } = [];
}
