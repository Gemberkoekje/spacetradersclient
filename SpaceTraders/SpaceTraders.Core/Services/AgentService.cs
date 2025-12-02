using SpaceTraders.Core.Helpers;
using SpaceTraders.Core.Models.AgentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class AgentService(Client.SpaceTradersService service)
{

    private Agent? Agent { get; set; }

    public event Action<Agent>? Updated;


    public async Task Initialize()
    {
        var response = await service.EnqueueAsync((client, ct) => client.GetMyAgentAsync(ct));
        Update(response.Value.Data);
    }

    public void Update(Client.Agent agent)
    {
        Update(MapAgent(agent));
    }

    private void Update(Agent agent)
    {
        Agent = agent;
        Updated?.Invoke(Agent);
    }

    public Agent? GetAgent()
    {
       return Agent;
    }

    private Agent MapAgent(Client.Agent agent)
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
