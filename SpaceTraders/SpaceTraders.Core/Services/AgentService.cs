using SpaceTraders.Core.Extensions;
using SpaceTraders.Core.Models.AgentModels;
using System;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Services;

public sealed class AgentService(Client.SpaceTradersService service)
{
    public async Task<Agent> GetAgent(string? agentSymbol = null)
    {
        if (agentSymbol == null)
        {
            var response = await service.EnqueueCachedAsync((client, ct) => client.GetMyAgentAsync(ct), "GetMyAgentAsync", TimeSpan.FromSeconds(10));
            return response.Value.Data.Convert();

        }
        else
        {
            var response = await service.EnqueueCachedAsync((client, ct) => client.GetAgentAsync(agentSymbol, ct), $"GetAgentAsync_{agentSymbol}", TimeSpan.FromSeconds(10));
            return response.Value.Data.Convert();
        }
    }
}
