using System;
using System.Collections.Generic;

namespace SpaceTraders.Core.Models.ContractModels;

public sealed record ContractTerms
{
    public DateTimeOffset Deadline { get; init; }
    public ContractPayment Payment { get; init; }
    public List<ContractDeliverGood> Deliver { get; init; } = [];
}
