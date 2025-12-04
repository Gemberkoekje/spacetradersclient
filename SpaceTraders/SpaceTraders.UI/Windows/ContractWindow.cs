using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ContractWindow : ClosableWindow
{
    private Contract? Contract { get; set; }

    public ContractWindow(RootScreen rootScreen, ContractService contractService)
        : base(rootScreen, 45, 30)
    {
        contractService.Updated += LoadData;
        DrawContent();
        LoadData(contractService.GetContracts().ToArray());
    }

    public void LoadData(Contract[] data)
    {
        if (Surface == null)
            return;
        var contract = data.Length > 0 ? data[0] : null;
        if (contract is null)
            return;
        if (Contract is not null && Contract.ContractsEqualByValue(Contract, contract))
            return;

        Title = $"Contract";
        Contract = contract;

        Binds["Faction"].SetData([$"{Contract.FactionSymbol}"]);
        Binds["Type"].SetData([$"{Contract.Type}"]);
        Binds["Terms.Deadline"].SetData([$"{Contract.Terms.Deadline}"]);
        Binds["Terms.Payment.OnAccepted"].SetData([$"{Contract.Terms.Payment.OnAccepted:#,###}"]);
        Binds["Terms.Payment.OnFulfilled"].SetData([$"{Contract.Terms.Payment.OnFulfilled:#,###}"]);
        Binds["DeliverList"].SetData(Contract.Terms.Deliver.Select(d => $"- Deliver {d.UnitsRequired} of {d.TradeSymbol} to {d.DestinationSymbol} (Fulfilled: {d.UnitsFulfilled})").ToArray());
        Binds["DeadlineToAccept"].SetData([$"{Contract.DeadlineToAccept}"]);
        Binds["Accepted"].SetData([$"{Contract.Accepted}"]);
        Binds["Fulfilled"].SetData([$"{Contract.Fulfilled}"]);
        ResizeAndRedraw();
    }

    private void DrawContent()
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
