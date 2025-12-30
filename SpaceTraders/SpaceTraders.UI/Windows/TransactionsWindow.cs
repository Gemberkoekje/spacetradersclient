using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class TransactionsWindow : ClosableWindow, ICanSetSymbols
{
    private string ParentSymbol { get; set; } = string.Empty;
    private string Symbol { get; set; } = string.Empty;

    private string AgentSymbol { get; set; } = string.Empty;

    private Shipyard? Shipyard { get; set; }

    private ShipyardService ShipyardService { get; init; }
    private CustomListBox<TransactionsListValue>? TransactionsListBox { get; set; }

    public TransactionsWindow(RootScreen rootScreen, ShipyardService shipyardService, AgentService agentService)
        : base(rootScreen, 52, 20)
    {
        ShipyardService = shipyardService;
        shipyardService.Updated += LoadData;
        AgentSymbol = agentService.GetAgent()?.Symbol ?? string.Empty;
        DrawContent();
    }

    public void SetSymbol(string[] symbols)
    {
        Symbol = symbols[0];
        ParentSymbol = symbols[1];
        LoadData(ShipyardService.GetShipyards());
    }

    public void LoadData(ImmutableDictionary<string, ImmutableList<Shipyard>> data)
    {
        if (Surface == null)
            return;
        var shipyard = data.GetValueOrDefault(ParentSymbol).First(s => s.Symbol == Symbol);

        Title = $"Shipyard {shipyard.Symbol}";
        Shipyard = shipyard;
        TransactionsListBox?.SetCustomData(Shipyard.Transactions.Select(t => new TransactionsListValue(t, AgentSymbol)).ToArray());

        ResizeAndRedraw();
    }

    private void DrawContent()
    {
        var y = 2;
        TransactionsListBox = Controls.AddListbox<TransactionsListValue>($"Transactions", 2, y, 80, 10);
        Binds.Add("Transactions", TransactionsListBox);
        y += 10;
    }

    private sealed class TransactionsListValue(Transaction transaction, string agentSymbol) : ColoredString(GetDisplayText(transaction), GetForegroundColor(transaction, agentSymbol), Color.Transparent)
    {
        public Transaction Transaction => transaction;

        private static string GetDisplayText(Transaction transaction)
        {
            return $"{transaction.Timestamp:u} - {transaction.WaypointSymbol} - {transaction.ShipType} - {transaction.Price} - {transaction.AgentSymbol}";
        }

        private static Color GetForegroundColor(Transaction transaction, string agentSymbol)
        {
            if (transaction.AgentSymbol == agentSymbol)
                return Color.Cyan;
            return Color.AnsiWhite;
        }
    }
}
