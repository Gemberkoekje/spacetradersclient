using Qowaiv;
using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ContractModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing contracts.
/// </summary>
public sealed class ContractService(Client.SpaceTradersService service)
{
    private ImmutableArray<Contract> Contracts { get; set; } = [];

    /// <summary>
    /// Event raised when contracts are updated.
    /// </summary>
    public event Action<ImmutableArray<Contract>>? Updated;

    /// <summary>
    /// Event raised when a contract deadline is approaching.
    /// </summary>
    public event Action<DateTimeOffset>? Expired;

    /// <summary>
    /// Initializes the contract service by fetching contracts.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Initialize()
    {
        var clientContracts = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetContractsAsync(page, limit, ct),
            page => page.Data);
        Update(clientContracts.Value.Select(MapContract));
    }

    /// <summary>
    /// Removes expired contracts.
    /// </summary>
    public void Expire()
    {
        if (Contracts.IsEmpty)
        {
            return;
        }

        foreach (var contract in Contracts.Where(c => !c.Accepted && c.DeadlineToAccept < Clock.UtcNow().AddSeconds(-1)))
        {
            Update(Contracts.Remove(contract));
        }

        foreach (var contract in Contracts.Where(c => c.Terms.Deadline < Clock.UtcNow().AddSeconds(-1)))
        {
            Update(Contracts.Remove(contract));
        }
    }

    /// <summary>
    /// Gets all contracts.
    /// </summary>
    /// <returns>The contracts.</returns>
    public ImmutableArray<Contract> GetContracts()
    {
        return Contracts;
    }

    /// <summary>
    /// Negotiates a new contract.
    /// </summary>
    /// <param name="shipSymbol">The ship symbol to use for negotiation.</param>
    /// <returns>A result containing the new contract.</returns>
    public async Task<Result<Contract>> NegotiateContract(ShipSymbol shipSymbol)
    {
        var result = await service.EnqueueAsync((client, ct) => client.NegotiateContractAsync(shipSymbol.ToString(), ct), true);
        if (result.IsValid)
        {
            Update(Contracts.Add(MapContract(result.Value.Data.Contract)));
        }

        return Result.WithMessages<Contract>(result.Messages);
    }

    private void Update(IEnumerable<Contract> contracts)
    {
        Contracts = [.. contracts];
        foreach (var contract in Contracts)
        {
            Expired?.Invoke(contract.DeadlineToAccept);
            Expired?.Invoke(contract.Terms.Deadline);
        }

        Updated?.Invoke(Contracts);
    }

    private static Contract MapContract(Client.Contract contract)
    {
        return new Contract()
        {
            Id = contract.Id,
            FactionSymbol = contract.FactionSymbol,
            Type = contract.Type.Convert<Client.ContractType, ContractType>(),
            Terms = new ContractTerms()
            {
                Deadline = contract.Terms.Deadline,
                Payment = new ContractPayment()
                {
                    OnAccepted = contract.Terms.Payment.OnAccepted,
                    OnFulfilled = contract.Terms.Payment.OnFulfilled,
                },
                Deliver = contract.Terms.Deliver.Select(d => new ContractDeliverGood()
                {
                    TradeSymbol = d.TradeSymbol,
                    DestinationSymbol = WaypointSymbol.Parse(d.DestinationSymbol),
                    UnitsRequired = d.UnitsRequired,
                }).ToImmutableHashSet(),
            },
            Accepted = contract.Accepted,
            Fulfilled = contract.Fulfilled,
            DeadlineToAccept = contract.DeadlineToAccept,
        };
    }
}
