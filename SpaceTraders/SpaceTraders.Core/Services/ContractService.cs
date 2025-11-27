using Qowaiv.Validation.Abstractions;
using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.ContractModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class ContractService(Client.SpaceTradersService service)
{
    public async Task<Contract[]> GetMyContracts()
    {
        var response = await service.EnqueueCachedAsync((client, ct) => client.GetContractsAsync(null, null, ct), "GetContractsAsync", TimeSpan.FromSeconds(10));
        var contracts = response.Value.Data;
        var result = contracts.Select(contract => MapContract(contract)).ToArray();
        return result;
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
                }).ToList(),
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
            return MapContract(result.Value.Data.Contract);
        }
        return Result.WithMessages<Contract>(result.Messages);
    }

}
