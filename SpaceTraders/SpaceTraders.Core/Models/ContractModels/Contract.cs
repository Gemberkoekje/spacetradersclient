using Qowaiv.Mathematics;
using SpaceTraders.Core.Enums;
using System;
using System.Numerics;

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

    public static bool ContractsEqualByValue(Contract a, Contract b)
    {
        if (Equals(a, b)) return true;
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        var ta = a.Terms;
        var tb = b.Terms;
        if (a.Id != b.Id) return false;
        if (a.FactionSymbol != b.FactionSymbol) return false;
        if (a.Type != b.Type) return false;
        if (a.Accepted != b.Accepted) return false;
        if (a.Fulfilled != b.Fulfilled) return false;
        if (a.DeadlineToAccept != b.DeadlineToAccept) return false;

        if (ta.Deadline != tb.Deadline) return false;
        if (ta.Payment.OnAccepted != tb.Payment.OnAccepted) return false;
        if (ta.Payment.OnFulfilled != tb.Payment.OnFulfilled) return false;

        // Ensure both sets use the same comparer and compare contents
        if (!ta.Deliver.SetEquals(tb.Deliver)) return false;

        return true;
    }
}
