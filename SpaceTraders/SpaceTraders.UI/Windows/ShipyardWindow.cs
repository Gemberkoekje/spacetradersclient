using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardWindow : DataBoundWindowWithContext<Shipyard, WaypointContext>
{
    private readonly ShipyardService _shipyardService;
    private CustomListBox<ShipyardShipListValue>? _shipyardShipListBox;

    public ShipyardWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;

        SubscribeToEvent<ImmutableDictionary<SystemSymbol, ImmutableArray<Shipyard>>>(
            handler => shipyardService.Updated += handler,
            handler => shipyardService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize();
    }

    protected override Shipyard? FetchData()
    {
        var shipyards = _shipyardService.GetShipyards().GetValueOrDefault(Context.System);
        return shipyards.IsDefault ? null : shipyards.FirstOrDefault(s => s.Symbol == Context.Waypoint);
    }

    protected override void BindData(Shipyard data)
    {
        Title = $"Shipyard {Context.Waypoint} in {Context.System}";
        Binds["Symbol"].SetData([$"{data.Symbol}"]);
        _shipyardShipListBox?.SetCustomData([.. data.Ships.Select(s => new ShipyardShipListValue(s.Type, s.Name, s.PurchasePrice))]);
        Binds["ModificationsFee"].SetData([$"{data.ModificationsFee}"]);
        Binds["Transactions"].SetData([$"Transactions ({data.Transactions.Count})"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"Shipyard.Symbol", 21, y++));

        _shipyardShipListBox = Controls.AddListbox<ShipyardShipListValue>($"ShipyardShips", 2, y, 80, 10);
        Binds.Add("ShipyardShips", _shipyardShipListBox);
        y += 10;
        Controls.AddButton("Show Ship", 2, y++, (_, _) => OpenShip());

        Controls.AddLabel($"Modifications fee:", 2, y);
        Binds.Add("ModificationsFee", Controls.AddLabel($"Shipyard.ModificationsFee", 21, y++));
        Binds.Add("Transactions", Controls.AddButton($"Shipyard.Transactions", 2, y, (_, _) => RootScreen.ShowWindow<TransactionsWindow, WaypointContext>(Context)));
    }

    private void OpenShip()
    {
        if (_shipyardShipListBox?.GetSelectedItem() is ShipyardShipListValue shipyardShip)
        {
            RootScreen.ShowWindow<ShipyardShipWindow, ShipyardShipContext>(new (shipyardShip.Type, Context.Waypoint, Context.System));
        }
    }

    private sealed record ShipyardShipListValue(ShipType Type, string Name, int PurchasePrice)
    {
        public override string ToString() => $"{Type} {Name} ({PurchasePrice:#,###})";
    }
}
