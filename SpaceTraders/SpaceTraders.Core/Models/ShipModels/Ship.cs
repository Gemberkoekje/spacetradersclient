using Qowaiv;
using SpaceTraders.Core.Enums;
using System;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Models.ShipModels;

/// <summary>
/// Aggregate root representing a SpaceTraders ship with identity, registration and navigation state.
/// </summary>
/// <remarks>
/// Composed of registration metadata and a navigation snapshot. Additional facets (cargo, modules, crew)
/// may be added as the domain model expands.
/// </remarks>
public sealed record Ship
{
    /// <summary>
    /// Unique ship symbol identifier.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Registration details (name, faction and role).
    /// </summary>
    required public Registration Registration { get; init; }

    /// <summary>
    /// Current navigation snapshot including route, positional status and flight mode.
    /// </summary>
    required public Navigation Navigation { get; init; }

    required public Crew Crew { get; init; }

    required public Frame Frame { get; init; }

    required public Reactor Reactor { get; init; }

    required public Engine Engine { get; init; }

    required public ImmutableList<Module> Modules { get; init; }

    required public ImmutableList<Mount> Mounts { get; init; }

    required public Cargo Cargo { get; init; }

    required public Fuel Fuel { get; init; }

    required public Cooldown Cooldown { get; init; }

    public bool CanDock => Navigation.Status == Enums.ShipNavStatus.InOrbit;

    public bool CanOrbit => Navigation.Status == Enums.ShipNavStatus.Docked;

    public bool CanNavigate => Navigation.Status == Enums.ShipNavStatus.InOrbit && Cooldown.RemainingSeconds == 0;

    public (int X, int Y)? Position
    {
        get
        {
            if (Navigation.Status != Enums.ShipNavStatus.InTransit)
                return (Navigation.Route.Destination.X, Navigation.Route.Destination.Y);

            var diffX = Navigation.Route.Destination.X - Navigation.Route.Origin.X;
            var diffY = Navigation.Route.Destination.Y - Navigation.Route.Origin.Y;
            var totalSeconds = (Navigation.Route.ArrivalTime - Navigation.Route.DepartureTime).TotalSeconds;
            var elapsedSeconds = (Clock.UtcNow() - Navigation.Route.DepartureTime).TotalSeconds;
            var ratio = elapsedSeconds / totalSeconds;
            var currentX = Navigation.Route.Origin.X + (int)(diffX * ratio);
            var currentY = Navigation.Route.Origin.Y + (int)(diffY * ratio);
            return (currentX, currentY);
        }
    }

    public Direction Direction
    {
        get
        {
            var deltaX = Navigation.Route.Destination.X - Navigation.Route.Origin.X;
            var deltaY = Navigation.Route.Destination.Y - Navigation.Route.Origin.Y;
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                return deltaX > 0 ? Direction.East : Direction.West;
            }
            else
            {
                return deltaY > 0 ? Direction.North : Direction.South;
            }
        }
    }
}
