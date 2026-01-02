using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class CargoWindow : DataBoundWindowWithContext<Cargo, ShipContext>
{
    private readonly ShipService _shipService;

    public CargoWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override Cargo? FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship)?.Cargo;

    protected override void BindData(Cargo data)
    {
        Title = $"Cargo for ship {Context.Ship}";
        Binds["CargoList"].SetData(data.Inventory.Select(c => $"{new string(' ', 5 - c.Units.ToString("#.###").Length)}{c.Units:#.###} {c.Name}").ToArray());
        Binds["Total"].SetData([$"{data.Units} / {data.Capacity}"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Binds.Add("CargoList", Controls.AddListbox($"CargoList", 20, y, 80, 10));
        y += 10;
        Controls.AddLabel($"Total:", "TotalLabel", 2, y);
        Binds.Add("Total", Controls.AddLabel($"Total", "Total", 20, y));
    }
}
