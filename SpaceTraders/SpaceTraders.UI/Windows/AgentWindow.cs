using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

public sealed class AgentWindow : ClosableWindow, ICanLoadData<Agent>
{
    private Agent? _agent { get; set; }

    public AgentWindow(RootScreen rootScreen)
        : base(rootScreen, 52, 11)
    {
        DrawContent();
    }

    public void LoadData(Agent data)
    {
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