using System;

namespace SpaceTraders.Core.Models.SystemModels;

/// <summary>
/// Represents a chart submission for a waypoint.
/// </summary>
public sealed record Chart
{
    /// <summary>
    /// Gets the waypoint symbol.
    /// </summary>
    required public string WaypointSymbol { get; init; }

    /// <summary>
    /// Gets the agent who submitted the chart.
    /// </summary>
    required public string SubmittedBy { get; init; }

    /// <summary>
    /// Gets the submission timestamp.
    /// </summary>
    required public DateTimeOffset SubmittedOn { get; init; }
}
