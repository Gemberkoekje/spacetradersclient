using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipWindow : ClosableWindow, ICanLoadData<Ship>
{
    string? ICanLoadData.Symbol { get; set; }

    private Ship? Ship { get; set; }

    private ContractService ContractService { get; init; }

    public ShipWindow(RootScreen rootScreen, ContractService contractService)
        : base(rootScreen, 52, 20)
    {
        ContractService = contractService;
        DrawContent();
    }

    public void LoadData(Ship data)
    {
        if (Ship is not null && Ship == data)
            return;

        Title = $"Ship {data.Symbol}";
        Ship = data;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Ship is null)
        {
            Controls.AddLabel($"Ship loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }
        var y = 2;
        Controls.AddLabel($"Symbol: {Ship.Symbol}", 2, y++);
        Controls.AddLabel($"Name: {Ship.Registration.Name}", 2, y++);
        Controls.AddLabel($"Faction: {Ship.Registration.FactionSymbol}", 2, y++);
        Controls.AddLabel($"Role: {Ship.Registration.Role}", 2, y++);
        Controls.AddLabel($"Navigation:", 2, y);
        Controls.AddButton($"{Ship.Navigation.Status} {(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? "to" : "at")} {Ship.Navigation.Route.Destination.Symbol}{(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? $" until {Ship.Navigation.Route.ArrivalTime}" : "")}", 15, y++, (_, _) => RootScreen.ShowWindow<NavigationWindow>(Ship.Symbol));
        Controls.AddButton($"Negotiate new contract", 2, y++, (_, _) => RootScreen.DoAsynchronousEventually(NegotiateNewContract));
        ResizeAndRedraw();
    }

    private async Task NegotiateNewContract()
    {
        var result = await ContractService.NegotiateContract(Ship!.Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWindow<ContractWindow>();
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }
}
