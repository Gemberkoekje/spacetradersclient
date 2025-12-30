using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// Window for displaying agent information.
/// </summary>
public sealed class AgentWindow : DataBoundWindowNoSymbols<Agent>
{
    private readonly AgentService _agentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentWindow"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="agentService">The agent service.</param>
    public AgentWindow(RootScreen rootScreen, AgentService agentService)
        : base(rootScreen, 52, 11)
    {
        _agentService = agentService;
        SubscribeToEvent<Agent?>(
            handler => agentService.Updated += handler,
            handler => agentService.Updated -= handler,
            OnServiceUpdatedSync);

        Initialize(refreshImmediately: true);
    }

    /// <inheritdoc/>
    protected override void DrawContent()
    {
        Controls.AddLabel($"Symbol:", "AgentSymbolLabel", 2, 2);
        Binds.Add("Symbol", Controls.AddLabel($"Symbol", "AgentSymbol", 20, 2));
        Controls.AddLabel($"ShipCount:", "AgentShipCountLabel", 2, 3);
        Binds.Add("ShipCount", Controls.AddLabel($"ShipCount", "AgentShipCount", 20, 3));
        Controls.AddLabel($"Headquarters:", "AgentHeadquartersLabel", 2, 4);
        Binds.Add("Headquarters", Controls.AddLabel($"Headquarters", "AgentHeadquarters", 20, 4));
        Controls.AddLabel($"Credits:", "AgentCreditsLabel", 2, 5);
        Binds.Add("Credits", Controls.AddLabel($"Credits", "AgentCredits", 20, 5));
        Controls.AddLabel($"Starting faction:", "AgentStartingFactionLabel", 2, 6);
        Binds.Add("StartingFaction", Controls.AddLabel($"Starting faction", "AgentStartingFaction", 20, 6));
    }

    /// <inheritdoc/>
    protected override Agent? FetchData() => _agentService.GetAgent();

    /// <inheritdoc/>
    protected override void BindData(Agent data)
    {
        Title = $"Agent: {data.Symbol}";
        Binds["Symbol"].SetData([$"{data.Symbol}"]);
        Binds["ShipCount"].SetData([$"{data.ShipCount}"]);
        Binds["Headquarters"].SetData([$"{data.Headquarters}"]);
        Binds["Credits"].SetData([$"{data.Credits:#,###}"]);
        Binds["StartingFaction"].SetData([$"{data.StartingFaction}"]);
    }
}
