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

    private MarketService MarketService { get; init; }

    public ShipWindow(RootScreen rootScreen, ContractService contractService, ShipService shipService, MarketService marketService)
        : base(rootScreen, 52, 20)
    {
        ContractService = contractService;
        ShipService = shipService;
        MarketService = marketService;
        DrawContent();
        shipService.Updated += LoadData;
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(ShipService.GetShips().ToArray()).GetAwaiter().GetResult();
    }

    public async Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return;
        var ship = data.First(s => s.Symbol == Symbol);
        if (Ship is not null && Ship == ship)
            return;

        Title = $"Ship {Symbol}";
        Ship = ship;
        Binds["Symbol"].SetData([$"{Ship.Symbol}"]);
        Binds["Registration.Name"].SetData([$"{Ship.Registration.Name}"]);
        Binds["Registration.FactionSymbol"].SetData([$"{Ship.Registration.FactionSymbol}"]);
        Binds["Registration.Role"].SetData([$"{Ship.Registration.Role} {Ship.Frame.Name}"]);
        Binds["Navigation.Status"].SetData([$"{Ship.Navigation.Status} {(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? "to" : "at")} {Ship.Navigation.Route.Destination.Symbol}{(Ship.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? $" until {Ship.Navigation.Route.ArrivalTime}" : "")}"]);
        Binds["Navigation.WaypointSymbol"].SetData([$"{Ship.Navigation.WaypointSymbol}"]);
        Binds["Cargo"].SetData([$"({Ship.Cargo.Units} / {Ship.Cargo.Capacity})"]);
        Binds["Fuel"].SetData([$"{Ship.Fuel.Current} / {Ship.Fuel.Capacity}{(Ship.Fuel.Consumed.Amount > 0 ? $" ({Ship.Fuel.Consumed.Amount} consumed at {Ship.Fuel.Consumed.Timestamp})" : "")}"]);
        Binds["Cooldown"].SetData([Ship.Cooldown.RemainingSeconds > 0 ? $"Cooldown: {Ship.Cooldown.RemainingSeconds} / {Ship.Cooldown.TotalSeconds} seconds{(Ship.Cooldown.RemainingSeconds > 0 ? $" (Expires at {Ship.Cooldown.Expiration})" : "")}" : "No Cooldown"]);
        Binds["Frame"].SetData([$"{Ship.Frame.Name}"]);
        Binds["Reactor"].SetData([$"{Ship.Reactor.Name} ({Ship.Reactor.PowerOutput} power)"]);
        Binds["Engine"].SetData([$"{Ship.Engine.Name} ({Ship.Engine.Speed} speed)"]);
        Binds["Modules"].SetData([$"Modules ({Ship.Modules.Count})"]);
        Binds["Mounts"].SetData([$"Mounts ({Ship.Mounts.Count})"]);
        Binds["Crew"].SetData([$"Crew ({Ship.Crew.Current} / {Ship.Crew.Capacity})"]);
        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"Ship.Symbol", 14, y++));
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Registration.Name", Controls.AddLabel($"Ship.Registration.Name", 14, y++));
        Controls.AddLabel($"Faction:", 2, y);
        Binds.Add("Registration.FactionSymbol", Controls.AddLabel($"Ship.Registration.FactionSymbol", 14, y++));
        Controls.AddLabel($"Role:", 2, y);
        Binds.Add("Registration.Role", Controls.AddLabel($"Ship.Registration.Role", 14, y++));
        y++;
        Controls.AddLabel($"Navigation:", 2, y);
        Binds.Add("Navigation.Status", Controls.AddButton($"Ship.Navigation.Status", 14, y++, (_, _) => RootScreen.ShowWindow<NavigationWindow>([Symbol])));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Navigation.WaypointSymbol", Controls.AddButton($"Ship.Navigation.WaypointSymbol", 14, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow>([Ship.Navigation.WaypointSymbol, Ship.Navigation.SystemSymbol])));
        Controls.AddLabel($"Cargo:", 2, y);
        Binds.Add("Cargo", Controls.AddButton($"Ship.Cargo", 14, y++, (_, _) => RootScreen.ShowWindow<CargoWindow>([Symbol])));
        Controls.AddLabel($"Fuel:", 2, y);
        Binds.Add("Fuel", Controls.AddLabel($"Ship.Fuel", 14, y));
        Controls.AddButton($"Refuel", 25, y++, (_, _) => RootScreen.ScheduleCommand(Refuel));
        Binds.Add("Cooldown", Controls.AddLabel($"Ship.Cooldown", 2, y++));
        y++;
        Binds.Add("Frame", Controls.AddButton($"Frame.Name", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow>([Ship.Frame.Symbol.ToString()])));
        Binds.Add("Reactor", Controls.AddButton($"Reactor.Name", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow>([Ship.Reactor.Symbol.ToString()])));
        Binds.Add("Engine", Controls.AddButton($"Engine.Name", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow>([Ship.Engine.Symbol.ToString()])));
        Binds.Add("Modules", Controls.AddButton($"Modules", 2, y++, (_, _) => RootScreen.ShowWindow<ModulesWindow>([Symbol])));
        Binds.Add("Mounts", Controls.AddButton($"Mounts", 2, y++, (_, _) => RootScreen.ShowWindow<MountsWindow>([Symbol])));
        Binds.Add("Crew", Controls.AddButton($"Crew", 2, y++, (_, _) => RootScreen.ShowWindow<CrewWindow>([Symbol])));

        y++;
        Controls.AddButton($"Negotiate new contract", 2, y++, (_, _) => RootScreen.ScheduleCommand(NegotiateNewContract));
    }

    private async Task Refuel()
    {
        var result = await MarketService.Refuel(Ship!.Symbol);
        if (!result.IsValid)
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private async Task NegotiateNewContract()
    {
        var result = await ContractService.NegotiateContract(Ship!.Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWindow<ContractWindow>([]);
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }
}
