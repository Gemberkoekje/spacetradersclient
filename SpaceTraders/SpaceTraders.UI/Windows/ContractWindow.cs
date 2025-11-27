using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

internal sealed class ContractWindow : ClosableWindow, ICanLoadData<Contract?>
{
    private Contract? Contract { get; set; }

    public ContractWindow(RootScreen rootScreen)
        : base(rootScreen, 45, 30)
    {
    }

    public void LoadData(Contract? data)
    {
        Title = $"Contract{(data != null ? $" {data.Id}" : string.Empty)}";
        Contract = data;
        DrawContent();
    }

    private void DrawContent()
    {
        if (Contract is null)
        {
            var y2 = 2;
            Controls.AddLabel($"No contract data available.", 2, y2++);
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
