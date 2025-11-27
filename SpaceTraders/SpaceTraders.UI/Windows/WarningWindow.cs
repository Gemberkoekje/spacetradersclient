using SpaceTraders.UI.Extensions;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.Windows;

internal sealed class WarningWindow : ClosableWindow, ICanLoadData<string>
{
    private string Warning { get; set; } = string.Empty;

    public WarningWindow(RootScreen rootScreen)
        : base(rootScreen, 52, 20)
    {
    }

    public void LoadData(string data)
    {
        Title = $"Warning";
        Warning = data;
        DrawContent();
    }

    private void DrawContent()
    {
        var y = 2;
        Controls.AddLabel($"{Warning}", 2, y++);
        ResizeAndRedraw();
    }
}
