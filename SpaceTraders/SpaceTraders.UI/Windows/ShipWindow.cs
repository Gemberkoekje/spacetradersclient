using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private Ship? Ship { get; set; }

    private ContractService ContractService { get; init; }

    private ShipService ShipService { get; init; }

    public ShipWindow(RootScreen rootScreen, ContractService contractService, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        ContractService = contractService;
        ShipService = shipService;
        shipService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string symbol, string? _)
    {
        Symbol = symbol;
        LoadData(ShipService.GetShips().ToArray());
    }

    public async Task LoadData(Ship[] data)
    {
        var ship = data.First(s => s.Symbol == Symbol);
        if (Ship is not null && Ship == ship)
            return;

        Title = $"Ship {ship.Symbol}";
        Ship = ship;
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
        Controls.AddButton($"Negotiate new contract", 2, y++, (_, _) => RootScreen.ScheduleCommand(NegotiateNewContract));
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
