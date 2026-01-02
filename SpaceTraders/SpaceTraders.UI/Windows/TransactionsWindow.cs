using SadConsole;
using SadRogue.Primitives;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipyardModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class TransactionsWindow : DataBoundWindowWithContext<Shipyard, WaypointContext>
{
    private readonly ShipyardService _shipyardService;
    private readonly AgentSymbol _agentSymbol;
    private CustomListBox<TransactionsListValue>? _transactionsListBox;

    public TransactionsWindow(RootScreen rootScreen, ShipyardService shipyardService, AgentService agentService)
        : base(rootScreen, 52, 20)
    {
        _shipyardService = shipyardService;
        _agentSymbol = agentService.GetAgent()?.Symbol ?? AgentSymbol.Empty;

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
        Title = $"Shipyard {data.Symbol}";
        _transactionsListBox?.SetCustomData(data.Transactions.Select(t => new TransactionsListValue(t, _agentSymbol)).ToArray());
    }

    protected override void DrawContent()
    {
        const int y = 2;
        _transactionsListBox = Controls.AddListbox<TransactionsListValue>($"Transactions", 2, y, 80, 10);
        Binds.Add("Transactions", _transactionsListBox);
    }

    private sealed class TransactionsListValue(Transaction transaction, AgentSymbol agentSymbol) : ColoredString(GetDisplayText(transaction), GetForegroundColor(transaction, agentSymbol), Color.Transparent)
    {
        private static string GetDisplayText(Transaction transaction)
        {
            return $"{transaction.Timestamp:u} - {transaction.WaypointSymbol} - {transaction.ShipType} - {transaction.Price} - {transaction.AgentSymbol}";
        }

        private static Color GetForegroundColor(Transaction transaction, AgentSymbol agentSymbol)
        {
            if (transaction.AgentSymbol == agentSymbol)
                return Color.Cyan;
            return Color.AnsiWhite;
        }
    }
}
