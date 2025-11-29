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

        Title = $"Ship {Symbol}";
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
        Controls.AddLabel($"Role: {Ship.Registration.Role} {Ship.Frame.Name}", 2, y++);
        y++;
        Controls.AddLabel($"Navigation:", 2, y);
        Controls.AddButton($"{Ship.Navigation.Status} {(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? "to" : "at")} {Ship.Navigation.Route.Destination.Symbol}{(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? $" until {Ship.Navigation.Route.ArrivalTime}" : "")}", 15, y++, (_, _) => RootScreen.ShowWindow<NavigationWindow>(Ship.Symbol));
        Controls.AddButton($"Cargo ({Ship.Cargo.Units} / {Ship.Cargo.Capacity})", 2, y++, (_, _) => RootScreen.ShowWindow<CargoWindow>(Ship.Symbol));
        Controls.AddLabel($"Fuel: {Ship.Fuel.Current} / {Ship.Fuel.Capacity}{(Ship.Fuel.Consumed.Amount > 0 ? $" ({Ship.Fuel.Consumed.Amount} consumed at {Ship.Fuel.Consumed.Timestamp})" : "")}", 2, y++);
        Controls.AddLabel($"Cooldown: {Ship.Cooldown.RemainingSeconds} / {Ship.Cooldown.TotalSeconds} seconds{(Ship.Cooldown.RemainingSeconds > 0 ? $" (Expires at {Ship.Cooldown.Expiration})" : "")}", 2, y++);
        y++;
        Controls.AddButton($"{Ship.Frame.Name}", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow>(Ship.Symbol));
        Controls.AddButton($"{Ship.Reactor.Name} ({Ship.Reactor.PowerOutput} power)", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow>(Ship.Symbol));
        Controls.AddButton($"{Ship.Engine.Name} ({Ship.Engine.Speed} speed)", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow>(Ship.Symbol));
        Controls.AddButton($"Modules ({Ship.Modules.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<ModulesWindow>(Ship.Symbol));
        Controls.AddButton($"Mounts ({Ship.Mounts.Count})", 2, y++, (_, _) => RootScreen.ShowWindow<MountsWindow>(Ship.Symbol));
        Controls.AddButton($"Crew ({Ship.Crew.Current} / {Ship.Crew.Capacity})", 2, y++, (_, _) => RootScreen.ShowWindow<CrewWindow>(Ship.Symbol));

        y++;
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
