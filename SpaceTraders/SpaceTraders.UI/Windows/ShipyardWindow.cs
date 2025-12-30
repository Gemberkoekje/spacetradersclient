using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ShipyardWindow : ClosableWindow, ICanSetSymbols
{
    private string SystemSymbol { get; set; } = string.Empty;
    private string WaypointSymbol { get; set; } = string.Empty;

    private Shipyard? Shipyard { get; set; }

    private ShipyardService ShipyardService { get; init; }

    private CustomListBox<ShipyardShipListValue> ShipyardShipListBox { get; set; }

    public ShipyardWindow(RootScreen rootScreen, ShipyardService shipyardService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        WaypointSymbol = symbols[0];
        SystemSymbol = symbols[1];
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        if (Surface == null)
            return;
        var shipyard = data.GetValueOrDefault(SystemSymbol)?.FirstOrDefault(s => s.Symbol == WaypointSymbol);
        if (shipyard is null)
            return;
        Title = $"Shipyard {WaypointSymbol} in {SystemSymbol}";
        Shipyard = shipyard;
        Binds["Symbol"].SetData([$"{Shipyard.Symbol}"]);
        ShipyardShipListBox.SetCustomData([.. Shipyard.Ships.Select(s => new ShipyardShipListValue(s.Type, s.Name, s.PurchasePrice))]);
        Binds["ModificationsFee"].SetData([$"{Shipyard.ModificationsFee}"]);
        Binds["Transactions"].SetData([$"Transactions ({Shipyard.Transactions.Count})"]);

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"Symbol:", 2, y);
        Binds.Add("Symbol", Controls.AddLabel($"Shipyard.Symbol", 21, y++));

        ShipyardShipListBox = Controls.AddListbox<ShipyardShipListValue>($"ShipyardShips", 2, y, 80, 10);
        Binds.Add("ShipyardShips", ShipyardShipListBox);
        y += 10;
        Controls.AddButton("Show Ship", 2, y++, (_, _) => OpenShip());

        Controls.AddLabel($"Modifications fee:", 2, y);
        Binds.Add("ModificationsFee", Controls.AddLabel($"Shipyard.ModificationsFee", 21, y++));
        Binds.Add("Transactions", Controls.AddButton($"Shipyard.Transactions", 2, y++, (_, _) => RootScreen.ShowWindow<TransactionsWindow>([WaypointSymbol, SystemSymbol])));
    }

    private void OpenShip()
    {
        var listbox = ShipyardShipListBox;
        if (listbox.GetSelectedItem() is ShipyardShipListValue shipyardShip)
        {
            RootScreen.ShowWindow<ShipyardShipWindow>([shipyardShip.Type.ToString(), WaypointSymbol, SystemSymbol]);
        }
    }

    private record ShipyardShipListValue(ShipType Type, string Name, int PurchasePrice)
    {
        public override string ToString() => $"{Type} {Name} ({PurchasePrice:#,###})";
    }
}
