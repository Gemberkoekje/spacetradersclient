using SpaceTraders.Core.Models.AgentModels;

namespace SpaceTraders.Core.Extensions;

internal static class AgentExtensions
{
    internal static Agent Convert(this Client.Agent agent)
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

    internal static Agent Convert(this Client.PublicAgent agent)
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
