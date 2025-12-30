using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemsWindow : ClosableWindow
{
    private SystemWaypoint[] Systems { get; set; }

    private Ship[] Ships { get; set; } = [];

    private CustomListBox<SystemListValue>? SystemsListBox { get; set; }

    public SystemsWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService)
        : base(rootScreen, 45, 30)
    {
        shipService.Updated += LoadData;
        systemService.Updated += LoadData;
        Title = "Known Systems";
        DrawContent();
        LoadData(systemService.GetSystems().ToArray());
        LoadData(shipService.GetShips().ToArray());
    }

    public void LoadData(SystemWaypoint[] data)
    {
        if (Surface == null)
            return;
        Systems = data;
        SystemsListBox.SetCustomData([.. Systems.Select(s => new SystemListValue(s, Ships.Count(sh => sh.Navigation.SystemSymbol == s.Symbol)))]);
        ResizeAndRedraw();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        Ships = data;
        SystemsListBox.SetCustomData([.. Systems.Select(s => new SystemListValue(s, Ships.Count(sh => sh.Navigation.SystemSymbol == s.Symbol)))]);
        ResizeAndRedraw();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        var y = 2;
        SystemsListBox = Controls.AddListbox<SystemListValue>($"Systems", 2, y, 80, 10);
        Binds.Add("Systems", SystemsListBox);
        y += 10;
        Controls.AddButton("Show System", 2, y++, (_, _) => OpenSystem());
    }

    private void OpenSystem()
    {
        var listbox = SystemsListBox;
        if (listbox.GetSelectedItem() is SystemListValue waypoint)
        {
            RootScreen.ShowWindow<SystemDataWindow>([waypoint.System.Symbol]);
        }
    }

    private sealed class SystemListValue(SystemWaypoint system, int shipCount) : ColoredString(GetDisplayText(system, shipCount), GetForegroundColor(shipCount), Color.Transparent)
    {
        public SystemWaypoint System => system;

        public int ShipCount => shipCount;

        private static string GetDisplayText(SystemWaypoint system, int shipCount)
        {
            return $"{system.Symbol} ({system.SystemType}) - Ships: {shipCount}";
        }

        private static Color GetForegroundColor(int shipCount)
        {
            if (shipCount > 0)
            {
                return Color.Cyan;
            }
            return Color.AnsiWhite;
        }
    }
}
