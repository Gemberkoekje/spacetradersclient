using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Client;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class ErrorData
{
    public string agentSymbol { get; set; }
    public string contractId { get; set; }
}

public class Error
{
    public int code { get; set; }
    public string message { get; set; }
    public ErrorData data { get; set; }
    public string requestId { get; set; }
}

public class ErrorResponse
{
    public Error error { get; set; }
}

