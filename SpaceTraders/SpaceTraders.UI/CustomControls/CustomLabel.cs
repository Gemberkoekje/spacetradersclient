using SadConsole.UI.Controls;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SpaceTraders.UI.CustomControls;

public class CustomLabel : Label, IHaveABottomRightCorner, ICanSetData
{
    public CustomLabel(int width) : base(width)
    {
    }

    public CustomLabel(string displayText) : base(displayText)
    {
    }

    public void SetData(string[] text)
    {
        DisplayText = text[0];
        Resize(ActualWidth, Height);
        IsDirty = true;
    }

    public int ActualWidth => DisplayText.Length;

    public (int X, int Y) BottomRightCorner => (Position.X + ActualWidth, Position.Y + Height);
}
