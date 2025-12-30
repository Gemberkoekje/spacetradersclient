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
    /// Gets the unique ship symbol identifier.
    /// </summary>
    required public string Symbol { get; init; }

    /// <summary>
    /// Gets the registration details (name, faction and role).
    /// </summary>
    required public Registration Registration { get; init; }

    /// <summary>
    /// Gets the current navigation snapshot including route, positional status and flight mode.
    /// </summary>
    required public Navigation Navigation { get; init; }

    /// <summary>
    /// Gets the crew information.
    /// </summary>
    required public Crew Crew { get; init; }

    /// <summary>
    /// Gets the ship frame.
    /// </summary>
    required public Frame Frame { get; init; }

    /// <summary>
    /// Gets the ship reactor.
    /// </summary>
    required public Reactor Reactor { get; init; }

    /// <summary>
    /// Gets the ship engine.
    /// </summary>
    required public Engine Engine { get; init; }

    /// <summary>
    /// Gets the ship modules.
    /// </summary>
    required public ImmutableArray<Module> Modules { get; init; }

    /// <summary>
    /// Gets the ship mounts.
    /// </summary>
    required public ImmutableArray<Mount> Mounts { get; init; }

    /// <summary>
    /// Gets the ship cargo.
    /// </summary>
    required public Cargo Cargo { get; init; }

    /// <summary>
    /// Gets the ship fuel.
    /// </summary>
    required public Fuel Fuel { get; init; }

    /// <summary>
    /// Gets the ship cooldown.
    /// </summary>
    required public Cooldown Cooldown { get; init; }

    /// <summary>
    /// Gets a value indicating whether the ship can dock.
    /// </summary>
    public bool CanDock => Navigation.Status == ShipNavStatus.InOrbit;

    /// <summary>
    /// Gets a value indicating whether the ship can orbit.
    /// </summary>
    public bool CanOrbit => Navigation.Status == ShipNavStatus.Docked;

    /// <summary>
    /// Gets a value indicating whether the ship can navigate.
    /// </summary>
    public bool CanNavigate => Navigation.Status == ShipNavStatus.InOrbit && Cooldown.RemainingSeconds == 0;

    /// <summary>
    /// Gets the current position of the ship.
    /// </summary>
    public (int X, int Y)? Position
    {
        get
        {
            if (Navigation.Status != ShipNavStatus.InTransit)
            {
                return (Navigation.Route.Destination.X, Navigation.Route.Destination.Y);
            }

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

    /// <summary>
    /// Gets the direction the ship is traveling.
    /// </summary>
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
