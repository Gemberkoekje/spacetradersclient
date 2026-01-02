using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class CrewWindow : DataBoundWindowWithContext<Crew, ShipContext>
{
    private readonly ShipService _shipService;

    public CrewWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override Crew? FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship)?.Crew;

    protected override void BindData(Crew data)
    {
        Title = $"Crew for ship {Context.Ship}";
        Binds["Current"].SetData([$"{data.Current}"]);
        Binds["Capacity"].SetData([$"{data.Capacity}"]);
        Binds["Rotation"].SetData([$"{data.Rotation}"]);
        Binds["Morale"].SetData([$"{data.Morale}"]);
        Binds["Wages"].SetData([$"{data.Wages}"]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Current Crew:", 2, y);
        Binds.Add("Current", Controls.AddLabel($"Crew.Current", 20, y++));
        Controls.AddLabel($"Crew Capacity:", 2, y);
        Binds.Add("Capacity", Controls.AddLabel($"Crew.Capacity", 20, y++));
        Controls.AddLabel($"Rotation:", 2, y);
        Binds.Add("Rotation", Controls.AddLabel($"Crew.Rotation", 20, y++));
        Controls.AddLabel($"Morale:", 2, y);
        Binds.Add("Morale", Controls.AddLabel($"Crew.Morale", 20, y++));
        Controls.AddLabel($"Wages:", 2, y);
        Binds.Add("Wages", Controls.AddLabel($"Crew.Wages", 20, y));
    }
}
