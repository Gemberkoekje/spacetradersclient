using Qowaiv;
using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.ContractModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class ContractService(Client.SpaceTradersService service)
{
    private ImmutableList<Contract> Contracts { get; set; } = [];

    public event Action<Contract[]>? Updated;

    public event Action<DateTimeOffset>? Expired;

    public async Task Initialize()
    {
        var clientContracts = await service.GetAllPagesAsync(
            (client, page, limit, ct) => client.GetContractsAsync(page, limit, ct),
            page => page.Data);
        Update(clientContracts.Value.Select(MapContract));
    }

    private void Update(IEnumerable<Contract> contracts)
    {
        Contracts = contracts.ToImmutableList();
        foreach (var contract in Contracts)
        {
            Expired?.Invoke(contract.DeadlineToAccept);
            Expired?.Invoke(contract.Terms.Deadline);
        }
        Updated?.Invoke(Contracts.ToArray());
    }

    public void Expire()
    {
        if (Contracts.IsEmpty)
            return;
        foreach (var contract in Contracts.Where(c => !c.Accepted && c.DeadlineToAccept < Clock.UtcNow().AddSeconds(-1)))
        {
            Update(Contracts.Remove(contract));
        }
        foreach (var contract in Contracts.Where(c => c.Terms.Deadline < Clock.UtcNow().AddSeconds(-1)))
        {
            Update(Contracts.Remove(contract));
        }
    }

    public ImmutableList<Contract> GetContracts()
    {
        return Contracts;
    }

    private static Contract MapContract(Client.Contract contract)
    {
        return new Contract()
        {
            Id = contract.Id,
            FactionSymbol = contract.FactionSymbol,
            Type = contract.Type.Convert(),
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
                    DestinationSymbol = d.DestinationSymbol,
                    UnitsRequired = d.UnitsRequired,
                }).ToImmutableHashSet(),
            },
            Accepted = contract.Accepted,
            Fulfilled = contract.Fulfilled,
            DeadlineToAccept = contract.DeadlineToAccept,
        };
    }

    public async Task<Result<Contract>> NegotiateContract(string shipSymbol)
    {
        var result = await service.EnqueueAsync((client, ct) => client.NegotiateContractAsync(shipSymbol, ct), true);
        if (result.IsValid)
        {
            Update(Contracts.Add(MapContract(result.Value.Data.Contract)));
        }
        return Result.WithMessages<Contract>(result.Messages);
    }

}
