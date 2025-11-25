using SadRogue.Primitives;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

public sealed class AgentWindow : ClosableWindow
{
    private readonly Agent _agent;

    public AgentWindow(Agent agent, RootScreen rootScreen)
        : base(rootScreen, 52, 11)
    {
        _agent = agent;
        Title = $"Agent: {agent.Symbol}";
        DrawContent();
    }

    private void DrawContent()
    {
        Controls.AddLabel($"Symbol: {_agent.Symbol}", 2, 2);
        Controls.AddLabel($"ShipCount: {_agent.ShipCount}", 2, 3);
        Controls.AddLabel($"Headquarters: {_agent.Headquarters}",2, 4);
        Controls.AddLabel($"Credits: {_agent.Credits}", 2, 5);
        Controls.AddLabel($"Starting faction: {_agent.StartingFaction}", 2, 6);
        ResizeAndRedraw();
    }
}