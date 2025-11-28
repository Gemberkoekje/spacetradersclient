using Qowaiv.Validation.Abstractions;
using SpaceTraders.UI.Extensions;
using System.Linq;

namespace SpaceTraders.UI.Windows;

internal sealed class WarningWindow : ClosableWindow
{
    private Result Data { get; set; }

    public WarningWindow(RootScreen rootScreen)
        : base(rootScreen, 52, 20)
    {
        DrawContent();
    }

    public void LoadData(Result data)
    {
        if (data.Errors.Any())
        {
            Title = $"Error";
        }
        else if (data.Warnings.Any())
        {
            Title = $"Warning";
        }
        else if (data.Infos.Any())
        {
            Title = $"Information";
        }
        Data = data;
        DrawContent();
    }

    private void DrawContent()
    {
        Clean();
        if (Data is null)
        {
            return;
        }

        var y = 2;
        foreach (var error in Data.Errors)
        {
            Controls.AddLabel($"{error.Message}", 2, y++);
        }
        foreach (var warning in Data.Warnings)
        {
            Controls.AddLabel($"{warning.Message}", 2, y++);
        }
        foreach (var info in Data.Infos)
        {
            Controls.AddLabel($"{info.Message}", 2, y++);
        }
        ResizeAndRedraw();
    }
}
