using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ContractWindow : ClosableWindow
{
    private Contract? Contract { get; set; }

    public ContractWindow(RootScreen rootScreen, ContractService contractService)
        : base(rootScreen, 45, 30)
    {
        contractService.Updated += LoadData;
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
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Contract is null)
        {
            if (!Loaded)
            {
                Controls.AddLabel($"Contract data loading...", 2, 2);
            }
            else
            {
                Controls.AddLabel($"No contract data available.", 2, 2);
            }
            ResizeAndRedraw();
            return;
        }

        var y = 2;
        Controls.AddLabel($"Contract ID: {Contract.Id}", 2, y++);
        Controls.AddLabel($"Faction: {Contract.FactionSymbol}", 2, y++);
        Controls.AddLabel($"Type: {Contract.Type}", 2, y++);
        Controls.AddLabel($"Terms Deadline: {Contract.Terms.Deadline}", 2, y++);
        Controls.AddLabel($"Payment on Accepted: {Contract.Terms.Payment.OnAccepted}", 2, y++);
        Controls.AddLabel($"Payment on Fulfilled: {Contract.Terms.Payment.OnFulfilled}", 2, y++);
        foreach (var delivery in Contract.Terms.Deliver)
        {
            Controls.AddLabel($"Deliver {delivery.UnitsRequired} of {delivery.TradeSymbol} to {delivery.DestinationSymbol} (Fulfilled: {delivery.UnitsFulfilled})", 2, y++);
        }
        Controls.AddLabel($"Deadline to accept: {Contract.DeadlineToAccept}", 2, y++);
        Controls.AddLabel($"Accepted: {Contract.Accepted}", 2, y++);
        Controls.AddLabel($"Fulfilled: {Contract.Fulfilled}", 2, y++);

        ResizeAndRedraw();
    }
}
