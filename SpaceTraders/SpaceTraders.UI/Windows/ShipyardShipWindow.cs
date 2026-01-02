using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardShipWindow : DataBoundWindowWithContext<ShipyardShip, ShipyardShipContext>
{
    private readonly ShipyardService _shipyardService;

    public ShipyardShipWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Shipyard>>>(
            handler => shipyardService.Updated += handler,
            handler => shipyardService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override ShipyardShip? FetchData()
    {
        var shipyards = _shipyardService.GetShipyards().GetValueOrDefault(Context.System);
        if (shipyards.IsDefault) return null;
        var shipyard = shipyards.FirstOrDefault(s => s.Symbol == Context.Waypoint);
        return shipyard?.Ships.FirstOrDefault(s => s.Type == Context.ShipType);
    }

    protected override void BindData(ShipyardShip data)
    {
        Title = $"{data.Type} {data.Frame.Name} at shipyard {Context.Waypoint}";
        Binds["PurchasePrice"].SetData([$"{data.PurchasePrice}"]);
        Binds["Activity"].SetData([$"{data.Activity}"]);
        Binds["Supply"].SetData([$"{data.Supply}"]);
        Binds["Name"].SetData([$"{data.Name}"]);
        Binds["Shipyard"].SetData([$"Shipyard {Context.Waypoint} in {Context.System}"]);
        Binds["Type"].SetData([$"{data.Type} {data.Frame.Name}"]);
        Binds["Navigation.Status"].SetData([$"Purchasable at {Context.Waypoint} in {Context.System}"]);
        Binds["Navigation.WaypointSymbol"].SetData([$"{Context.Waypoint}"]);
        Binds["Cargo"].SetData([$"(0 / {data.CargoCapacity})"]);
        Binds["Fuel"].SetData([$"{data.Frame.FuelCapacity} / {data.Frame.FuelCapacity}"]);
        Binds["Cooldown"].SetData(["No Cooldown"]);
        Binds["Frame"].SetData([$"{data.Frame.Name}"]);
        Binds["Reactor"].SetData([$"{data.Reactor.Name} ({data.Reactor.PowerOutput} power)"]);
        Binds["Engine"].SetData([$"{data.Engine.Name} ({data.Engine.Speed} speed)"]);
        Binds["Modules"].SetData([$"Modules ({data.Modules.Length})"]);
        Binds["Mounts"].SetData([$"Mounts ({data.Mounts.Length})"]);
        Binds["Crew"].SetData([$"Crew ({data.Crew.Required} / {data.Crew.Capacity})"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Price:", 2, y);
        Binds.Add("PurchasePrice", Controls.AddLabel($"Ship.PurchasePrice", 14, y++));
        Controls.AddLabel($"Activity:", 2, y);
        Binds.Add("Activity", Controls.AddLabel($"Ship.Activity", 14, y++));
        Controls.AddLabel($"Supply:", 2, y);
        Binds.Add("Supply", Controls.AddLabel($"Ship.Supply", 14, y++));
        y++;

        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"Ship.Name", 14, y++));
        Controls.AddLabel($"Faction:", 2, y);
        Binds.Add("Shipyard", Controls.AddLabel($"Ship.Shipyard", 14, y++));
        Controls.AddLabel($"Role:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"Ship.Type", 14, y++));
        y++;
        Controls.AddLabel($"Navigation:", 2, y);
        Binds.Add("Navigation.Status", Controls.AddLabel($"Ship.Navigation.Status", 14, y++));
        Controls.AddLabel($"Waypoint:", 2, y);
        Binds.Add("Navigation.WaypointSymbol", Controls.AddButton($"Ship.Navigation.WaypointSymbol", 14, y++, (_, _) => RootScreen.ShowWindow<WaypointWindow, WaypointContext>(new (Context.Waypoint, Context.System))));
        Controls.AddLabel($"Cargo:", 2, y);
        Binds.Add("Cargo", Controls.AddLabel($"Ship.Cargo", 14, y++));
        Controls.AddLabel($"Fuel:", 2, y);
        Binds.Add("Fuel", Controls.AddLabel($"Ship.Fuel", 14, y++));
        Binds.Add("Cooldown", Controls.AddLabel($"Ship.Cooldown", 2, y++));
        y++;
        Binds.Add("Frame", Controls.AddButton($"Frame.Name", 2, y++, (_, _) => RootScreen.ShowWindow<FrameWindow, FrameContext>(new (CurrentData!.Frame.Symbol))));
        Binds.Add("Reactor", Controls.AddButton($"Reactor.Name", 2, y++, (_, _) => RootScreen.ShowWindow<ReactorWindow, ReactorContext>(new (CurrentData!.Reactor.Symbol))));
        Binds.Add("Engine", Controls.AddButton($"Engine.Name", 2, y++, (_, _) => RootScreen.ShowWindow<EngineWindow, EngineContext>(new (CurrentData!.Engine.Symbol))));
        Binds.Add("Modules", Controls.AddButton($"Modules", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardModulesWindow, ShipyardShipContext>(Context)));
        Binds.Add("Mounts", Controls.AddButton($"Mounts", 2, y++, (_, _) => RootScreen.ShowWindow<ShipyardMountsWindow, ShipyardShipContext>(Context)));
        Binds.Add("Crew", Controls.AddLabel($"Crew", 4, y));
    }
}
