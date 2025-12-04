using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;
using System.Linq;

namespace SpaceTraders.UI.CustomControls;

public class CustomListBox: ListBox, IHaveABottomRightCorner, ICanSetData
{
    public CustomListBox(int width, int height) : base(width, height)
    {
    }

    public void SetData(string[] items)
    {
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
        Resize(ActualWidth, Height);
        IsDirty = true;
    }

    public int ActualWidth => Items.Any() ? Items.Max(i => (i.ToString() ?? "").Length + 2) : 35;


    public (int X, int Y) BottomRightCorner => (Position.X + Width, Position.Y + Height);
}
