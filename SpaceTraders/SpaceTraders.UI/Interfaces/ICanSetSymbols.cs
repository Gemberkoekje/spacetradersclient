using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.UI.Interfaces
{
    internal interface ICanSetSymbols
    {
        public void SetSymbol(string symbol, string? parentSymbol = null);
    }
}
