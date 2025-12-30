using SpaceTraders.Core.Models.AgentModels;
using System;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing agent data.
/// </summary>
public sealed class AgentService(Client.SpaceTradersService service)
{
    private Agent? Agent { get; set; }

    /// <summary>
    /// Event raised when the agent data is updated.
    /// </summary>
    public event Action<Agent>? Updated;

    /// <summary>
    /// Initializes the agent service by fetching agent data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Initialize()
    {
        var response = await service.EnqueueAsync((client, ct) => client.GetMyAgentAsync(ct));
        Update(response.Value.Data);
    }

    /// <summary>
    /// Updates the agent data from the client model.
    /// </summary>
    /// <param name="agent">The client agent model.</param>
    public void Update(Client.Agent agent)
    {
        Update(MapAgent(agent));
    }

    /// <summary>
    /// Gets the current agent.
    /// </summary>
    /// <returns>The current agent or null.</returns>
    public Agent? GetAgent()
    {
        return Agent;
    }

    private void Update(Agent agent)
    {
        Agent = agent;
        Updated?.Invoke(Agent);
    }

    private static Agent MapAgent(Client.Agent agent)
    {
        return new Agent
        {
            Symbol = agent.Symbol,
            Headquarters = agent.Headquarters,
            Credits = agent.Credits,
            StartingFaction = agent.StartingFaction,
            ShipCount = agent.ShipCount,
        };
    }
}
