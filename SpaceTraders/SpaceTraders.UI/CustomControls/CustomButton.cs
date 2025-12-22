using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;

namespace SpaceTraders.UI.CustomControls;

public sealed class CustomButton : Button, IHaveABottomRightCorner, ICanSetData
{
    public CustomButton(int width) : base(width)
    {
    }

    public CustomButton(string text) : base(text)
    {
    }

    public void SetData(object[] text)
    {
        Text = text[0]?.ToString() ?? string.Empty;
        //Resize(ActualWidth, Height);
        IsDirty = true;
    }

    public int ActualWidth => Text.Length + 4;

    public (int X, int Y) BottomRightCorner => (Position.X + ActualWidth, Position.Y + Height);
}
