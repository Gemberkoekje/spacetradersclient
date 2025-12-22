using SadConsole;
using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;
using System.Linq;

namespace SpaceTraders.UI.CustomControls;

public class CustomListBox: ListBox, IHaveABottomRightCorner, ICanSetData
{
    private bool Resize { get; init; }

    public CustomListBox(int width, int height, bool resize = true) : base(width, height)
    {
        Resize = resize;
    }

    public void SetData(object[] items)
    {
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
        if (Resize)
        {
            Resize(ActualWidth, Height);
        }
        IsDirty = true;
    }

    public int ActualWidth => Items.Any() ? Items.Max(i => (i.ToString() ?? "").Length + 2) : 35;


    public (int X, int Y) BottomRightCorner => (Position.X + Width, Position.Y + Height);
}
