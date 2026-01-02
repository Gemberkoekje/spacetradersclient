using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipsWindow : DataBoundWindowNoSymbols<ImmutableArray<Ship>>
{
    private readonly ShipService _shipService;
    private CustomListBox<ShipListValue>? _shipsListBox;

    public ShipsWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 11)
    {
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize(refreshImmediately: true);
    }

    protected override ImmutableArray<Ship> FetchData() => _shipService.GetShips();

    protected override bool DataEquals(ImmutableArray<Ship> current, ImmutableArray<Ship> previous)
    {
        if (current.IsDefault && previous.IsDefault) return true;
        if (current.IsDefault || previous.IsDefault) return false;
        return current.All(s => s == previous.FirstOrDefault(d => d.Symbol == s.Symbol))
            && previous.All(s => s == current.FirstOrDefault(d => d.Symbol == s.Symbol));
    }

    protected override void BindData(ImmutableArray<Ship> data)
    {
        Title = $"Ships";
        _shipsListBox?.SetCustomData([.. data.Select(ship => new ShipListValue(ship))]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        _shipsListBox = Controls.AddListbox<ShipListValue>($"Ships", 2, y, 80, 10);
        Binds.Add("Ships", _shipsListBox);
        y += 10;
        Controls.AddButton("Show Ship", 2, y, (_, _) => OpenShip());
    }

    private void OpenShip()
    {
        if (_shipsListBox?.GetSelectedItem() is ShipListValue shipListValue)
        {
            RootScreen.ShowWindow<ShipWindow, ShipContext>(new (shipListValue.Ship.Symbol));
        }
    }

    private sealed record ShipListValue(Ship Ship)
    {
        public override string ToString() => $"{Ship.Registration.Name} ({Ship.Registration.Role} {Ship.Frame.Name} at {Ship.Navigation.WaypointSymbol} in {Ship.Navigation.SystemSymbol})";
    }
}
