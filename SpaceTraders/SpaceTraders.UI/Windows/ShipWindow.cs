using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipWindow : DataBoundWindowWithContext<Ship, ShipContext>
{
    private readonly ContractService _contractService;
    private readonly ShipService _shipService;
    private readonly MarketService _marketService;

    public ShipWindow(RootScreen rootScreen, ContractService contractService, ShipService shipService, MarketService marketService)
        : base(rootScreen, 52, 20)
    {
        _contractService = contractService;
        _shipService = shipService;
        _marketService = marketService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override Ship? FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship);

    protected override void BindData(Ship data)
    {
        Title = $"Ship {Context.Ship}";
        Binds["Symbol"].SetData([$"{data.Symbol}"]);
        Binds["Registration.Name"].SetData([$"{data.Registration.Name}"]);
        Binds["Registration.FactionSymbol"].SetData([$"{data.Registration.FactionSymbol}"]);
        Binds["Registration.Role"].SetData([$"{data.Registration.Role} {data.Frame.Name}"]);
        Binds["Navigation.Status"].SetData([$"{data.Navigation.Status} {(data.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? "to" : "at")} {data.Navigation.Route.Destination.Symbol}{(data.Navigation.Status == Core.Enums.ShipNavStatus.InTransit ? $" until {data.Navigation.Route.ArrivalTime}" : string.Empty)}"]);
        Binds["Navigation.WaypointSymbol"].SetData([$"{data.Navigation.WaypointSymbol}"]);
        Binds["Cargo"].SetData([$"({data.Cargo.Units} / {data.Cargo.Capacity})"]);
        Binds["Fuel"].SetData([$"{data.Fuel.Current} / {data.Fuel.Capacity}{(data.Fuel.Consumed.Amount > 0 ? $" ({data.Fuel.Consumed.Amount} consumed at {data.Fuel.Consumed.Timestamp})" : string.Empty)}"]);
        var cooldownExpires = data.Cooldown.RemainingSeconds > 0 ? $" (Expires at {data.Cooldown.Expiration})" : string.Empty;
        Binds["Cooldown"].SetData([data.Cooldown.RemainingSeconds > 0 ? $"Cooldown: {data.Cooldown.RemainingSeconds} / {data.Cooldown.TotalSeconds} seconds{cooldownExpires}" : "No Cooldown"]);
        Binds["Frame"].SetData([$"{data.Frame.Name}"]);
        Binds["Reactor"].SetData([$"{data.Reactor.Name} ({data.Reactor.PowerOutput} power)"]);
        Binds["Engine"].SetData([$"{data.Engine.Name} ({data.Engine.Speed} speed)"]);
        Binds["Modules"].SetData([$"Modules ({data.Modules.Length})"]);
        Binds["Mounts"].SetData([$"Mounts ({data.Mounts.Length})"]);
        Binds["Crew"].SetData([$"Crew ({data.Crew.Current} / {data.Crew.Capacity})"]);
    }

    protected override void DrawContent()
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
        Binds.Add("Navigation.Status", Controls.AddButton($"Ship.Navigation.Status", 14, y++, (_, _) => RootScreen.ShowWindow<NavigationWindow, ShipContext>(Context)));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Navigation.WaypointSymbol", Controls.AddButton($"Ship.Navigation.WaypointSymbol", 14, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow, WaypointContext>(new (CurrentData!.Navigation.WaypointSymbol, CurrentData.Navigation.SystemSymbol))));
        Controls.AddLabel($"Cargo:", 2, y);
        Binds.Add("Cargo", Controls.AddButton($"Ship.Cargo", 14, y++, (_, _) => RootScreen.ShowWindow<CargoWindow, ShipContext>(Context)));
        Controls.AddLabel($"Fuel:", 2, y);
        Binds.Add("Fuel", Controls.AddLabel($"Ship.Fuel", 14, y));
        Controls.AddButton($"Refuel", 25, y++, (_, _) => RootScreen.ScheduleCommand(Refuel));
        Binds.Add("Cooldown", Controls.AddLabel($"Ship.Cooldown", 2, y++));
        y++;
        Binds.Add("Frame", Controls.AddButton($"Frame.Name", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow, FrameContext>(new (CurrentData!.Frame.Symbol))));
        Binds.Add("Reactor", Controls.AddButton($"Reactor.Name", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow, ReactorContext>(new (CurrentData!.Reactor.Symbol))));
        Binds.Add("Engine", Controls.AddButton($"Engine.Name", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow, EngineContext>(new (CurrentData!.Engine.Symbol))));
        Binds.Add("Modules", Controls.AddButton($"Modules", 2, y++, (_, _) => RootScreen.ShowWindow<ModulesWindow, ShipContext>(Context)));
        Binds.Add("Mounts", Controls.AddButton($"Mounts", 2, y++, (_, _) => RootScreen.ShowWindow<MountsWindow, ShipContext>(Context)));
        Binds.Add("Crew", Controls.AddButton($"Crew", 2, y++, (_, _) => RootScreen.ShowWindow<CrewWindow, ShipContext>(Context)));

        y++;
        Controls.AddButton($"Negotiate new contract", 2, y, (_, _) => RootScreen.ScheduleCommand(NegotiateNewContract));
    }

    private async Task Refuel()
    {
        var result = await _marketService.Refuel(CurrentData!.Symbol);
        if (!result.IsValid)
        {
            RootScreen.ShowWarningWindow(result);
        }
    }

    private async Task NegotiateNewContract()
    {
        var result = await _contractService.NegotiateContract(CurrentData!.Symbol);
        if (result.IsValid)
        {
            RootScreen.ShowWindow<ContractWindow, Contract>();
        }
        else
        {
            RootScreen.ShowWarningWindow(result);
        }
    }
}
