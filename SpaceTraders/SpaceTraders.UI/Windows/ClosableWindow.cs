using SadConsole.UI.Controls;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// A window that can be closed by clicking a close button.
/// </summary>
public abstract class ClosableWindow : AutoResizableWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosableWindow"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="width">The initial width.</param>
    /// <param name="height">The initial height.</param>
    protected ClosableWindow(RootScreen rootScreen, int width, int height)
        : base(rootScreen, width, height)
    {
        var closeButton = new Button(3)
        {
            Name = "CloseButton",
            Text = "X",
            Position = (Width - 4, 0),
        };
        closeButton.Click += (_, _) => RootScreen.HideAndDestroyWindow(this);
        Controls.Add(closeButton);
        Center();
    }
}
