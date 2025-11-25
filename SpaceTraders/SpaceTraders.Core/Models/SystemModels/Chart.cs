using System;

namespace SpaceTraders.Core.Models.SystemModels;

public sealed record Chart
{
    required public string WaypointSymbol { get; init; }
    required public string SubmittedBy { get; init; }
    required public DateTimeOffset SubmittedOn { get; init; }
}
