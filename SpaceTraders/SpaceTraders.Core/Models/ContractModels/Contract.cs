using SpaceTraders.Core.Enums;
using System;

namespace SpaceTraders.Core.Models.ContractModels;

/// <summary>
/// Represents a contract in the game.
/// </summary>
public sealed record Contract
{
    /// <summary>
    /// Gets the contract identifier.
    /// </summary>
    required public string Id { get; init; }

    /// <summary>
    /// Gets the faction symbol associated with the contract.
    /// </summary>
    required public string FactionSymbol { get; init; }

    /// <summary>
    /// Gets the contract type.
    /// </summary>
    public ContractType Type { get; init; }

    /// <summary>
    /// Gets the contract terms.
    /// </summary>
    required public ContractTerms Terms { get; init; }

    /// <summary>
    /// Gets a value indicating whether the contract has been accepted.
    /// </summary>
    public bool Accepted { get; init; }

    /// <summary>
    /// Gets a value indicating whether the contract has been fulfilled.
    /// </summary>
    public bool Fulfilled { get; init; }

    /// <summary>
    /// Gets the deadline to accept the contract.
    /// </summary>
    public DateTimeOffset DeadlineToAccept { get; init; }

    /// <summary>
    /// Compares two contracts for value equality.
    /// </summary>
    /// <param name="a">The first contract.</param>
    /// <param name="b">The second contract.</param>
    /// <returns>True if the contracts are equal by value; otherwise, false.</returns>
    public static bool ContractsEqualByValue(Contract a, Contract b)
    {
        if (Equals(a, b))
        {
            return true;
        }

        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        var ta = a.Terms;
        var tb = b.Terms;
        if (a.Id != b.Id)
        {
            return false;
        }

        if (a.FactionSymbol != b.FactionSymbol)
        {
            return false;
        }

        if (a.Type != b.Type)
        {
            return false;
        }

        if (a.Accepted != b.Accepted)
        {
            return false;
        }

        if (a.Fulfilled != b.Fulfilled)
        {
            return false;
        }

        if (a.DeadlineToAccept != b.DeadlineToAccept)
        {
            return false;
        }

        if (ta.Deadline != tb.Deadline)
        {
            return false;
        }

        if (ta.Payment.OnAccepted != tb.Payment.OnAccepted)
        {
            return false;
        }

        if (ta.Payment.OnFulfilled != tb.Payment.OnFulfilled)
        {
            return false;
        }

        // Ensure both sets use the same comparer and compare contents
        if (!ta.Deliver.SetEquals(tb.Deliver))
        {
            return false;
        }

        return true;
    }
}
