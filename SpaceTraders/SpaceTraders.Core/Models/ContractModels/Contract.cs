using SpaceTraders.Core.Enums;
using System;

namespace SpaceTraders.Core.Models.ContractModels;

public sealed record Contract
{
    public string Id { get; init; }
    public string FactionSymbol { get; init; }
    public ContractType Type { get; init; }
    public ContractTerms Terms { get; init; }
    public bool Accepted { get; init; }
    public bool Fulfilled { get; init; }
    public DateTimeOffset DeadlineToAccept { get; init; }
}
