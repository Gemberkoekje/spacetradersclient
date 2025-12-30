using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ContractWindow : DataBoundWindowNoSymbols<Contract>
{
    private readonly ContractService _contractService;

    public ContractWindow(RootScreen rootScreen, ContractService contractService)
        : base(rootScreen, 45, 30)
    {
        _contractService = contractService;

        SubscribeToEvent<ImmutableArray<Contract>>(
            handler => contractService.Updated += handler,
            handler => contractService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize(refreshImmediately: true);
    }

    protected override Contract? FetchData() =>
        _contractService.GetContracts().FirstOrDefault();

    protected override bool DataEquals(Contract? current, Contract? previous)
    {
        if (current is null && previous is null) return true;
        if (current is null || previous is null) return false;
        return Contract.ContractsEqualByValue(current, previous);
    }

    protected override void BindData(Contract data)
    {
        Title = $"Contract";
        Binds["Faction"].SetData([$"{data.FactionSymbol}"]);
        Binds["Type"].SetData([$"{data.Type}"]);
        Binds["Terms.Deadline"].SetData([$"{data.Terms.Deadline}"]);
        Binds["Terms.Payment.OnAccepted"].SetData([$"{data.Terms.Payment.OnAccepted:#,###}"]);
        Binds["Terms.Payment.OnFulfilled"].SetData([$"{data.Terms.Payment.OnFulfilled:#,###}"]);
        Binds["DeliverList"].SetData(data.Terms.Deliver.Select(d => $"- Deliver {d.UnitsRequired} of {d.TradeSymbol} to {d.DestinationSymbol} (Fulfilled: {d.UnitsFulfilled})").ToArray());
        Binds["DeadlineToAccept"].SetData([$"{data.DeadlineToAccept}"]);
        Binds["Accepted"].SetData([$"{data.Accepted}"]);
        Binds["Fulfilled"].SetData([$"{data.Fulfilled}"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Faction:", "FactionLabel", 2, y);
        Binds.Add("Faction", Controls.AddLabel($"Faction", "Faction", 25, y++));
        Controls.AddLabel($"Type:", "TypeLabel", 2, y);
        Binds.Add("Type", Controls.AddLabel($"Type", "Type", 25, y++));
        Controls.AddLabel($"Terms Deadline:", "Terms.DeadlineLabel", 2, y);
        Binds.Add("Terms.Deadline", Controls.AddLabel($"Terms.Deadline", "Terms.Deadline", 25, y++));
        Controls.AddLabel($"Payment on Accepted:", "Terms.Payment.OnAcceptedLabel", 2, y);
        Binds.Add("Terms.Payment.OnAccepted", Controls.AddLabel($"Terms.Payment.OnAccepted", "Terms.Payment.OnAccepted", 25, y++));
        Controls.AddLabel($"Payment on Fulfilled:", "Terms.Payment.OnFulfilledLabel", 2, y);
        Binds.Add("Terms.Payment.OnFulfilled", Controls.AddLabel($"Terms.Payment.OnFulfilled", "Terms.Payment.OnFulfilled", 25, y++));
        Controls.AddLabel($"Deadline to accept:", "DeadlineToAcceptLabel", 2, y);
        Binds.Add("DeadlineToAccept", Controls.AddLabel($"DeadlineToAccept", "DeadlineToAccept", 25, y++));
        Controls.AddLabel($"Accepted:", "Accepted", 2, y);
        Binds.Add("Accepted", Controls.AddLabel($"Accepted", "Accepted", 25, y++));
        Controls.AddLabel($"Fulfilled:", "Fulfilled", 2, y);
        Binds.Add("Fulfilled", Controls.AddLabel($"Fulfilled", "Fulfilled", 25, y++));
        Controls.AddLabel($"Deliverables:", "DeliverLabel", 2, y++);
        Binds.Add("DeliverList", Controls.AddListbox($"DeliverList", 2, y, 35, 10));
    }
}
