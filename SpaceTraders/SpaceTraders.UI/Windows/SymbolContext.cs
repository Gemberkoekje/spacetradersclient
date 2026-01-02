using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// Marker interface for symbol contexts used by data-bound windows.
/// </summary>
public interface ISymbolContext { }

/// <summary>
/// Context for windows that display ship-related data.
/// </summary>
/// <param name="Ship">The ship symbol.</param>
public readonly record struct ShipContext(ShipSymbol Ship) : ISymbolContext;

/// <summary>
/// Context for windows that display frame-related data.
/// </summary>
/// <param name="Frame">The frame symbol.</param>
public readonly record struct FrameContext(FrameSymbol Frame) : ISymbolContext;

/// <summary>
/// Context for windows that display reactor-related data.
/// </summary>
/// <param name="Reactor">The reactor symbol.</param>
public readonly record struct ReactorContext(ReactorSymbol Reactor) : ISymbolContext;

/// <summary>
/// Context for windows that display engine-related data.
/// </summary>
/// <param name="Engine">The engine symbol.</param>
public readonly record struct EngineContext(EngineSymbol Engine) : ISymbolContext;

/// <summary>
/// Context for windows that display module-related data.
/// </summary>
/// <param name="Module">The module symbol.</param>
public readonly record struct ModuleContext(ModuleSymbol Module) : ISymbolContext;

/// <summary>
/// Context for windows that display mount-related data.
/// </summary>
/// <param name="Mount">The mount symbol.</param>
public readonly record struct MountContext(MountSymbol Mount) : ISymbolContext;

/// <summary>
/// Context for windows that display system-related data.
/// </summary>
/// <param name="System">The system symbol.</param>
public readonly record struct SystemContext(SystemSymbol System) : ISymbolContext;

/// <summary>
/// Context for windows that display waypoint data within a system.
/// </summary>
/// <param name="Waypoint">The waypoint symbol.</param>
/// <param name="System">The system symbol.</param>
public readonly record struct WaypointContext(WaypointSymbol Waypoint, SystemSymbol System) : ISymbolContext;

/// <summary>
/// Context for windows that display ship type data at a shipyard.
/// </summary>
/// <param name="ShipType">The ship type.</param>
/// <param name="Waypoint">The waypoint symbol where the shipyard is located.</param>
/// <param name="System">The system symbol.</param>
public readonly record struct ShipyardShipContext(ShipType ShipType, WaypointSymbol Waypoint, SystemSymbol System) : ISymbolContext;
