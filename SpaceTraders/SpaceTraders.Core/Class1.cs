using SpaceTraders.Client;
using System.Net.Http;

namespace SpaceTraders.Core;

public sealed class Class1
{
    public Class1()
    {
        var client = new SpaceTradersClient(new HttpClient());
        var x = client.GetSystemAsync("SYMBOL").GetAwaiter().GetResult();
    }
}
