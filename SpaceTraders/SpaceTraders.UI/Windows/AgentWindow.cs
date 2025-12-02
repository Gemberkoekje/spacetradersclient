using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Extensions;

namespace SpaceTraders.UI.Windows;

public sealed class AgentWindow : ClosableWindow
{
    private Agent? _agent { get; set; }

    private AgentService AgentService { get; init; }

    public AgentWindow(RootScreen rootScreen, AgentService agentService)
        : base(rootScreen, 52, 11)
    {
        AgentService = agentService;
        AgentService.Updated += LoadData;
        LoadData(agentService.GetAgent());
    }

    public void LoadData(Agent data)
    {
        if (Surface == null)
            return;

        if (_agent is not null && _agent == data)
            return;

        _agent = data;
        Title = $"Agent: {data.Symbol}";
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (_agent is null)
        {
            Controls.AddLabel($"Agent data loading...", 2, 2);
            ResizeAndRedraw();
            return;
        }

        Controls.AddLabel($"Symbol: {_agent.Symbol}", 2, 2);
        Controls.AddLabel($"ShipCount: {_agent.ShipCount}", 2, 3);
        Controls.AddLabel($"Headquarters: {_agent.Headquarters}",2, 4);
        Controls.AddLabel($"Credits: {_agent.Credits}", 2, 5);
        Controls.AddLabel($"Starting faction: {_agent.StartingFaction}", 2, 6);
        ResizeAndRedraw();
    }
}