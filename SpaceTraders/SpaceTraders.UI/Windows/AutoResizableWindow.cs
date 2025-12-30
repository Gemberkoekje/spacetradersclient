using SadConsole.UI;
using SpaceTraders.UI.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// A window that can be resized automatically based on its content.
/// </summary>
public abstract class AutoResizableWindow : Window
{
    /// <summary>
    /// Gets the data bindings dictionary.
    /// </summary>
    protected Dictionary<string, ICanSetData> Binds { get; init; } = new ();

    /// <summary>
    /// Gets the root screen.
    /// </summary>
    protected RootScreen RootScreen { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the window has been loaded.
    /// </summary>
    protected bool Loaded { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResizableWindow"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="width">The initial width.</param>
    /// <param name="height">The initial height.</param>
    protected AutoResizableWindow(RootScreen rootScreen, int width, int height)
        : base(width, height)
    {
        RootScreen = rootScreen;

        CanDrag = true;

        Center();
    }

    /// <summary>
    /// Resizes the window based on content and redraws.
    /// </summary>
    protected void ResizeAndRedraw()
    {
        var maxwidth = Controls.Where(c => c is IHaveABottomRightCorner).Max(c => ((IHaveABottomRightCorner)c).BottomRightCorner.X);
        var maxheight = Controls.Where(c => c is IHaveABottomRightCorner).Max(c => ((IHaveABottomRightCorner)c).BottomRightCorner.Y);

        Resize(maxwidth + 2, maxheight + 2, true);
        var closebutton = Controls.SingleOrDefault(c => c.Name == "CloseButton");
        if (closebutton != null)
        {
            closebutton.Position = (Width - 4, 0);
        }
        DrawBorder();
        if (!Loaded)
        {
            Loaded = true;
            Center();
        }
        if (Position.Y < 0)
        {
            Position = (Position.X, 0);
        }
        IsDirty = true;
    }

    /// <summary>
    /// Cleans the window by removing all controls except the close button.
    /// </summary>
    protected void Clean()
    {
        foreach (var c in Controls.Where(c => c.Name != "CloseButton").ToList())
        {
            Controls.Remove(c);
        }
    }
}
