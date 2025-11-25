using SpaceTraders.Core.Enums;
using System;

namespace SpaceTraders.Core.Extensions;

/// <summary>
/// Extension conversion helpers for mapping generated client enum values to internal domain enums.
/// </summary>
/// <remarks>
/// The generated client (namespace <c>Client</c>) uses uppercase snake-case enum members. These helpers translate them
/// into the idiomatic C# enum declarations used in the core library. An <see cref="ArgumentOutOfRangeException"/> is thrown
/// when an unexpected enum value is encountered, making additions explicit during upgrades.
/// </remarks>
public static class EnumExtensions
{
    /// <summary>
    /// Converts a generated client ship role value to the internal <see cref="ShipRole"/> enum.
    /// </summary>
    /// <param name="shipRole">The client enum value to convert.</param>
    /// <returns>The corresponding internal <see cref="ShipRole"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="shipRole"/> is not a recognized value.</exception>
    public static ShipRole Convert(this Client.ShipRole shipRole)
    {
        return shipRole switch
        {
            Client.ShipRole.FABRICATOR => ShipRole.Fabricator,
            Client.ShipRole.HARVESTER => ShipRole.Harvester,
            Client.ShipRole.HAULER => ShipRole.Hauler,
            Client.ShipRole.INTERCEPTOR => ShipRole.Interceptor,
            Client.ShipRole.EXCAVATOR => ShipRole.Excavator,
            Client.ShipRole.TRANSPORT => ShipRole.Transport,
            Client.ShipRole.REPAIR => ShipRole.Repair,
            Client.ShipRole.SURVEYOR => ShipRole.Surveyor,
            Client.ShipRole.COMMAND => ShipRole.Command,
            Client.ShipRole.CARRIER => ShipRole.Carrier,
            Client.ShipRole.PATROL => ShipRole.Patrol,
            Client.ShipRole.SATELLITE => ShipRole.Satellite,
            Client.ShipRole.EXPLORER => ShipRole.Explorer,
            Client.ShipRole.REFINERY => ShipRole.Refinery,
            _ => throw new ArgumentOutOfRangeException(nameof(shipRole), $"Not expected ship role value: {shipRole}"),
        };
    }

    /// <summary>
    /// Converts a generated client navigation status value to the internal <see cref="ShipNavStatus"/> enum.
    /// </summary>
    /// <param name="shipNavStatus">The client enum value to convert.</param>
    /// <returns>The corresponding internal <see cref="ShipNavStatus"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="shipNavStatus"/> is not a recognized value.</exception>
    public static ShipNavStatus Convert(this Client.ShipNavStatus shipNavStatus)
    {
        return shipNavStatus switch
        {
            Client.ShipNavStatus.IN_TRANSIT => ShipNavStatus.InTransit,
            Client.ShipNavStatus.IN_ORBIT => ShipNavStatus.InOrbit,
            Client.ShipNavStatus.DOCKED => ShipNavStatus.Docked,
            _ => throw new ArgumentOutOfRangeException(nameof(shipNavStatus), $"Not expected ship nav status value: {shipNavStatus}"),
        };
    }

    /// <summary>
    /// Converts a generated client flight mode value to the internal <see cref="ShipNavFlightMode"/> enum.
    /// </summary>
    /// <param name="shipNavFlightMode">The client enum value to convert.</param>
    /// <returns>The corresponding internal <see cref="ShipNavFlightMode"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="shipNavFlightMode"/> is not a recognized value.</exception>
    public static ShipNavFlightMode Convert(this Client.ShipNavFlightMode shipNavFlightMode)
    {
        return shipNavFlightMode switch
        {
            Client.ShipNavFlightMode.DRIFT => ShipNavFlightMode.Drift,
            Client.ShipNavFlightMode.STEALTH => ShipNavFlightMode.Stealth,
            Client.ShipNavFlightMode.CRUISE => ShipNavFlightMode.Cruise,
            Client.ShipNavFlightMode.BURN => ShipNavFlightMode.Burn,
            _ => throw new ArgumentOutOfRangeException(nameof(shipNavFlightMode), $"Not expected ship nav flight mode value: {shipNavFlightMode}"),
        };
    }

    /// <summary>
    /// Converts a generated client waypoint type value to the internal <see cref="WaypointType"/> enum.
    /// </summary>
    /// <param name="waypointType">The client enum value to convert.</param>
    /// <returns>The corresponding internal <see cref="WaypointType"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="waypointType"/> is not a recognized value.</exception>
    public static WaypointType Convert(this Client.WaypointType waypointType)
    {
        return waypointType switch
        {
            Client.WaypointType.PLANET => WaypointType.Planet,
            Client.WaypointType.GAS_GIANT => WaypointType.GasGiant,
            Client.WaypointType.MOON => WaypointType.Moon,
            Client.WaypointType.ORBITAL_STATION => WaypointType.OrbitalStation,
            Client.WaypointType.JUMP_GATE => WaypointType.JumpGate,
            Client.WaypointType.ASTEROID_FIELD => WaypointType.AsteroidField,
            Client.WaypointType.ASTEROID => WaypointType.Asteroid,
            Client.WaypointType.ENGINEERED_ASTEROID => WaypointType.EngineeredAsteroid,
            Client.WaypointType.ASTEROID_BASE => WaypointType.AsteroidBase,
            Client.WaypointType.NEBULA => WaypointType.Nebula,
            Client.WaypointType.DEBRIS_FIELD => WaypointType.DebrisField,
            Client.WaypointType.GRAVITY_WELL => WaypointType.GravityWell,
            Client.WaypointType.ARTIFICIAL_GRAVITY_WELL => WaypointType.ArtificialGravityWell,
            Client.WaypointType.FUEL_STATION => WaypointType.FuelStation,
            _ => throw new ArgumentOutOfRangeException(nameof(waypointType), $"Not expected waypoint type value: {waypointType}"),
        };
    }

    public static SystemType Convert(this Client.SystemType systemType)
    {
        return systemType switch
        {
            Client.SystemType.NEUTRON_STAR => SystemType.NeutronStar,
            Client.SystemType.RED_STAR => SystemType.RedStar,
            Client.SystemType.ORANGE_STAR => SystemType.OrangeStar,
            Client.SystemType.BLUE_STAR => SystemType.BlueStar,
            Client.SystemType.YOUNG_STAR => SystemType.YoungStar,
            Client.SystemType.WHITE_DWARF => SystemType.WhiteDwarf,
            Client.SystemType.BLACK_HOLE => SystemType.BlackHole,
            Client.SystemType.HYPERGIANT => SystemType.HyperGiant,
            Client.SystemType.NEBULA => SystemType.Nebula,
            Client.SystemType.UNSTABLE => SystemType.Unstable,
            _ => throw new ArgumentOutOfRangeException(nameof(systemType), $"Not expected system type value: {systemType}"),
        };
    }

    public static FactionSymbol Convert(this Client.FactionSymbol systemType)
    {
        return systemType switch
        {
            Client.FactionSymbol.COSMIC => FactionSymbol.Cosmic,
            Client.FactionSymbol.VOID => FactionSymbol.Void,
            Client.FactionSymbol.GALACTIC => FactionSymbol.Galactic,
            Client.FactionSymbol.QUANTUM => FactionSymbol.Quantum,
            Client.FactionSymbol.DOMINION => FactionSymbol.Dominion,
            Client.FactionSymbol.ASTRO => FactionSymbol.Astro,
            Client.FactionSymbol.CORSAIRS => FactionSymbol.Corsairs,
            Client.FactionSymbol.OBSIDIAN => FactionSymbol.Obsidian,
            Client.FactionSymbol.AEGIS => FactionSymbol.Aegis,
            Client.FactionSymbol.UNITED => FactionSymbol.United,
            Client.FactionSymbol.SOLITARY => FactionSymbol.Solitary,
            Client.FactionSymbol.COBALT => FactionSymbol.Cobalt,
            Client.FactionSymbol.OMEGA => FactionSymbol.Omega,
            Client.FactionSymbol.ECHO => FactionSymbol.Echo,
            Client.FactionSymbol.LORDS => FactionSymbol.Lords,
            Client.FactionSymbol.CULT => FactionSymbol.Cult,
            Client.FactionSymbol.ANCIENTS => FactionSymbol.Ancients,
            Client.FactionSymbol.SHADOW => FactionSymbol.Shadow,
            Client.FactionSymbol.ETHEREAL => FactionSymbol.Etheral,
            _ => throw new ArgumentOutOfRangeException(nameof(systemType), $"Not expected system type value: {systemType}"),
        };
    }
    public static WaypointModifierSymbol Convert(this Client.WaypointModifierSymbol waypointModifierSymbol)
    {
        return waypointModifierSymbol switch
        {
            Client.WaypointModifierSymbol.STRIPPED => WaypointModifierSymbol.Stripped,
            Client.WaypointModifierSymbol.UNSTABLE => WaypointModifierSymbol.Unstable,
            Client.WaypointModifierSymbol.RADIATION_LEAK => WaypointModifierSymbol.RadiationLeak,
            Client.WaypointModifierSymbol.CRITICAL_LIMIT => WaypointModifierSymbol.CriticalLimit,
            Client.WaypointModifierSymbol.CIVIL_UNREST => WaypointModifierSymbol.CivilUnrest,
            _ => throw new ArgumentOutOfRangeException(nameof(waypointModifierSymbol), $"Not expected waypoint modifier symbol value: {waypointModifierSymbol}"),
        };
    }

    public static WaypointTraitSymbol Convert(this Client.WaypointTraitSymbol waypointTraitSymbol)
    {
        // Many values exist (69+). Convert generically from UPPER_SNAKE_CASE to PascalCase and map to internal enum.
        var name = waypointTraitSymbol.ToString();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentOutOfRangeException(nameof(waypointTraitSymbol), $"Not expected waypoint trait symbol value: {waypointTraitSymbol}");

        // Build PascalCase (split on '_', lowercase each segment, capitalize first char) then concatenate
        Span<char> buffer = stackalloc char[name.Length];
        int write = 0;
        bool capitalize = true;
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (c == '_')
            {
                capitalize = true;
                continue;
            }

            char lower = char.ToLowerInvariant(c);
            if (capitalize)
            {
                buffer[write++] = char.ToUpperInvariant(lower);
                capitalize = false;
            }
            else
            {
                buffer[write++] = lower;
            }
        }

        var pascalName = new string(buffer[..write]);
        if (Enum.TryParse<WaypointTraitSymbol>(pascalName, out var result))
        {
            return result;
        }

        throw new ArgumentOutOfRangeException(nameof(waypointTraitSymbol), $"Not expected waypoint trait symbol value: {waypointTraitSymbol}");
    }
}
