using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemsWindow : ClosableWindow
{
    private SystemWaypoint[] Systems { get; set; }

    private Ship[] Ships { get; set; } = [];

    public SystemsWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        shipService.Updated += LoadData;
        systemService.Updated += LoadData;
        Title = "Known Systems";
        LoadData(systemService.GetSystems().ToArray());
        LoadData(shipService.GetShips().ToArray());
    }

    public void LoadData(SystemWaypoint[] data)
    {
        if (Surface == null)
            return;
        Systems = data;
        DrawContent();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        Ships = data;
        DrawContent();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        Clean();
        if (Systems == null || !Systems.Any())
        {
            Controls.AddLabel($"No systems loaded.", 2, 2);
            ResizeAndRedraw();
            return;
        }

        var y = 2;
        foreach (var system in Systems)
        {
            var shipsInSystem = Ships.Where(s => s.Navigation.SystemSymbol == system.Symbol).ToArray();
            Controls.AddButton($"{system.Symbol} - {system.SystemType} (Ships: {shipsInSystem.Length})", 2, y++, (_, _) => RootScreen.ShowWindow<SystemDataWindow>([system.Symbol]));
        }

        ResizeAndRedraw();
    }
}
