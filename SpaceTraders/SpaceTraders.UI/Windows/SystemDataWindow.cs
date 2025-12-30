using Microsoft.Xna.Framework.Media;
using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Models.SystemModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class SystemDataWindow : ClosableWindow, ICanSetSymbols
{
    private string Symbol { get; set; } = string.Empty;

    private SystemWaypoint? System { get; set; }

    private Waypoint[] Waypoints { get; set; } = [];

    private CustomListBox<WaypointListValue> WaypointListBox { get; set; }

    private Ship[] Ships { get; set; } = [];

    private readonly ShipService ShipService;
    private readonly SystemService SystemService;
    private readonly WaypointService WaypointService;

    public SystemDataWindow(RootScreen rootScreen, ShipService shipService, SystemService systemService, WaypointService waypointService)
        : base(rootScreen, 45, 30)
    {
        shipService.Updated += LoadData;
        systemService.Updated += LoadData;
        waypointService.Updated += LoadData;
        ShipService = shipService;
        SystemService = systemService;
        WaypointService = waypointService;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        LoadData(SystemService.GetSystems().ToArray());
        LoadData(WaypointService.GetWaypoints());
        LoadData(ShipService.GetShips().ToArray());
    }

    public void LoadData(SystemWaypoint[] data)
    {
        if (Surface == null)
            return;
        var system = data.FirstOrDefault(d => d.Symbol == Symbol);
        if (System is not null && System == system)
            return;
        if (system is null)
            return;
        System = system;
        Title = $"System {System.Name}";
        RebindAndResize();
    }

    public Task LoadData(Ship[] data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var relevantData = data.Where(d => d.Navigation.SystemSymbol == System.Symbol).ToArray();
        if (Ships.All(s => s == relevantData.FirstOrDefault(d => d.Symbol == s.Symbol)) && relevantData.All(s => s == Ships.FirstOrDefault(d => d.Symbol == s.Symbol)))
            return Task.CompletedTask;
        Ships = relevantData;
        RebindAndResize();
        return Task.CompletedTask;
    }

    private void RebindAndResize()
    {
        if (System is null)
            return;
        Binds["Symbol"].SetData([$"{System.Symbol}"]);
        Binds["Name"].SetData([$"{System.Name}"]);
        Binds["Constellation"].SetData([$"{System.Constellation}"]);
        Binds["Sector"].SetData([$"{System.SectorSymbol}"]);
        Binds["Type"].SetData([$"{System.SystemType}"]);
        Binds["Position"].SetData([$"{System.X:0,000}, {System.Y:0,000}"]);
        Binds["Waypoints"].SetData([$"{Waypoints.Count()}"]);
        Binds["Factions"].SetData([$"{System.Factions.Count()}"]);

        var waypointdata = new List<WaypointListValue>();

        foreach (var wp in Waypoints.Where(wp => wp.Type != Core.Enums.WaypointType.Asteroid).OrderBy(w => w.Symbol))
        {
            waypointdata.Add(new WaypointListValue(wp, null, Ships.Any(s => s.Navigation.WaypointSymbol == wp.Symbol)));
            foreach (var ship in Ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol))
            {
                waypointdata.Add(new WaypointListValue(wp, ship, true));
            }
        }
        foreach (var wp in Waypoints.Where(wp => wp.Type == Core.Enums.WaypointType.Asteroid).OrderBy(w => w.Symbol))
        {
            waypointdata.Add(new WaypointListValue(wp, null, Ships.Any(s => s.Navigation.WaypointSymbol == wp.Symbol)));
            foreach (var ship in Ships.Where(s => s.Navigation.WaypointSymbol == wp.Symbol))
            {
                waypointdata.Add(new WaypointListValue(wp, ship, true));
            }
        }
        WaypointListBox.SetCustomData([.. waypointdata]);

        ResizeAndRedraw();
    }

    public Task LoadData(ImmutableDictionary<string, ImmutableList<Waypoint>> data)
    {
        if (Surface == null)
            return Task.CompletedTask;
        var waypoints = data.GetValueOrDefault(Symbol);
        if (waypoints is null)
            return Task.CompletedTask;
        Waypoints = waypoints.ToArray();
        RebindAndResize();
        return Task.CompletedTask;
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"System.Symbol", 21, y++));
        Controls.AddLabel($"Name:", 2, y);
        Binds.Add("Name", Controls.AddLabel($"System.Name", 21, y++));
        Controls.AddLabel($"Constellation:", 2, y);
        Binds.Add("Constellation", Controls.AddLabel($"System.Constellation", 21, y++));
        Controls.AddLabel($"Sector:", 2, y);
        Binds.Add("Sector", Controls.AddLabel($"System.SectorSymbol", 21, y++));
        Controls.AddLabel($"Type:", 2, y);
        Binds.Add("Type", Controls.AddLabel($"System.SystemType", 21, y++));
        Controls.AddLabel($"Position:", 2, y);
        Binds.Add("Position", Controls.AddLabel($"System.X System.Y", 21, y++));
        Controls.AddLabel($"Waypoints:", 2, y);
        Binds.Add("Waypoints", Controls.AddLabel($"System.Waypoints.Count", 21, y++));
        Controls.AddLabel($"Factions:", 2, y);
        Binds.Add("Factions", Controls.AddLabel($"System.Factions.Count", 21, y++));
        Controls.AddButton($"Show map", 2, y++, (_, _) => RootScreen.ShowWindow<SystemMapWindow>([System.Symbol]));
        y++;

        Controls.AddLabel($"Markets & Shipyards:", 2, y++);
        WaypointListBox = Controls.AddListbox<WaypointListValue>($"WaypointList", 2, y, 80, 25);
        Binds.Add("WaypointList", WaypointListBox);
        y += 25;
        Controls.AddButton("Show Waypoint", 2, y++, (_, _) => OpenWaypoint());
    }

    private void OpenWaypoint()
    {
        var listbox = WaypointListBox;
        if (listbox.GetSelectedItem() is WaypointListValue waypoint)
        {
            if (waypoint.Ship is not null)
            {
                RootScreen.ShowWindow<ShipWindow>([waypoint.Ship.Symbol]);
                return;
            }
            RootScreen.ShowWindow<WaypointWindow>([waypoint.Waypoint.Symbol, waypoint.Waypoint.SystemSymbol]);
        }
    }

    private sealed class WaypointListValue(Waypoint wp, Ship? ship, bool hasShipsDocked) : ColoredString(GetDisplayText(wp, ship), GetForegroundColor(wp, ship, hasShipsDocked), Color.Transparent)
    {
        public Waypoint Waypoint => wp;

        public Ship? Ship => ship;

        private static string GetDisplayText(Waypoint wp, Ship? ship)
        {
            int indent = 0;
            if (ship != null)
            {
                indent += 1;
            }
            if (!string.IsNullOrEmpty(wp.Orbits))
            {
                indent += 1;
            }
            var substring = indent > 0 ? $"{new string(' ', indent - 1)}- " : string.Empty;
            if (ship != null)
            {
                return $"{substring}{ship.Symbol} ({ship.Registration.Role})";
            }
            return $"{substring}{wp.Symbol} ({wp.Type}){(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace) ? " Marketplace" : string.Empty)}{(wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Shipyard) ? " & Shipyard" : string.Empty)}";
        }

        private static Color GetForegroundColor(Waypoint wp, Ship? ship, bool hasShipsDocked)
        {
            if (ship != null)
            {
                return Color.WhiteSmoke;
            }
            if (hasShipsDocked)
            {
                return Color.Cyan;
            }
            if (wp.Traits.Any(t => t.Symbol == Core.Enums.WaypointTraitSymbol.Marketplace))
            {
                return Color.MediumSeaGreen;
            }
            if (wp.Type == Core.Enums.WaypointType.Asteroid)
            {
                return Color.Gray;
            }
            return Color.AnsiWhite;
        }
    }
}
