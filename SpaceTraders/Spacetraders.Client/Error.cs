using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceTraders.Client;

/// <summary>
/// Contains additional data about an error.
/// </summary>
public sealed class ErrorData
{
    /// <summary>
    /// Gets the agent symbol associated with the error.
    /// </summary>
    public string agentSymbol { get; init; } = string.Empty;

    /// <summary>
    /// Gets the contract ID associated with the error.
    /// </summary>
    public string contractId { get; init; } = string.Empty;
}

/// <summary>
/// Represents an API error.
/// </summary>
public sealed class Error
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public int code { get; init; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string message { get; init; } = string.Empty;

    /// <summary>
    /// Gets additional error data.
    /// </summary>
    public ErrorData data { get; init; } = new ();

    /// <summary>
    /// Gets the request ID.
    /// </summary>
    public string requestId { get; init; } = string.Empty;
}

/// <summary>
/// Represents an error response from the API.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    /// Gets the error details.
    /// </summary>
    public Error error { get; init; } = new ();
}
