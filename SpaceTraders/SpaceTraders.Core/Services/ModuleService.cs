using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SpaceTraders.Core.Services;

/// <summary>
/// Service for managing ship modules, frames, reactors, engines, and mounts.
/// </summary>
public sealed class ModuleService
{
    private ImmutableDictionary<FrameSymbol, Frame> Frames { get; set; } = ImmutableDictionary<FrameSymbol, Frame>.Empty;

    private ImmutableDictionary<ReactorSymbol, Reactor> Reactors { get; set; } = ImmutableDictionary<ReactorSymbol, Reactor>.Empty;

    private ImmutableDictionary<EngineSymbol, Engine> Engines { get; set; } = ImmutableDictionary<EngineSymbol, Engine>.Empty;

    private ImmutableDictionary<ModuleSymbol, Module> Modules { get; set; } = ImmutableDictionary<ModuleSymbol, Module>.Empty;

    private ImmutableDictionary<MountSymbol, Mount> Mounts { get; set; } = ImmutableDictionary<MountSymbol, Mount>.Empty;

    /// <summary>
    /// Gets all frames.
    /// </summary>
    /// <returns>The frames dictionary.</returns>
    public ImmutableDictionary<FrameSymbol, Frame> GetFrames()
    {
        return Frames;
    }

    /// <summary>
    /// Gets all reactors.
    /// </summary>
    /// <returns>The reactors dictionary.</returns>
    public ImmutableDictionary<ReactorSymbol, Reactor> GetReactors()
    {
        return Reactors;
    }

    /// <summary>
    /// Gets all engines.
    /// </summary>
    /// <returns>The engines dictionary.</returns>
    public ImmutableDictionary<EngineSymbol, Engine> GetEngines()
    {
        return Engines;
    }

    /// <summary>
    /// Gets all modules.
    /// </summary>
    /// <returns>The modules dictionary.</returns>
    public ImmutableDictionary<ModuleSymbol, Module> GetModules()
    {
        return Modules;
    }

    /// <summary>
    /// Gets all mounts.
    /// </summary>
    /// <returns>The mounts dictionary.</returns>
    public ImmutableDictionary<MountSymbol, Mount> GetMounts()
    {
        return Mounts;
    }

    /// <summary>
    /// Adds frames to the collection.
    /// </summary>
    /// <param name="frames">The frames to add.</param>
    public void AddFrames(ImmutableArray<Frame> frames)
    {
        Frames = Frames.SetItems(frames.Select(f => new KeyValuePair<FrameSymbol, Frame>(f.Symbol, f)));
    }

    /// <summary>
    /// Adds reactors to the collection.
    /// </summary>
    /// <param name="reactors">The reactors to add.</param>
    public void AddReactors(ImmutableArray<Reactor> reactors)
    {
        Reactors = Reactors.SetItems(reactors.Select(r => new KeyValuePair<ReactorSymbol, Reactor>(r.Symbol, r)));
    }

    /// <summary>
    /// Adds engines to the collection.
    /// </summary>
    /// <param name="engines">The engines to add.</param>
    public void AddEngines(ImmutableArray<Engine> engines)
    {
        Engines = Engines.SetItems(engines.Select(e => new KeyValuePair<EngineSymbol, Engine>(e.Symbol, e)));
    }

    /// <summary>
    /// Adds modules to the collection.
    /// </summary>
    /// <param name="modules">The modules to add.</param>
    public void AddModules(ImmutableArray<Module> modules)
    {
        Modules = Modules.SetItems(modules.Select(m => new KeyValuePair<ModuleSymbol, Module>(m.Symbol, m)));
    }

    /// <summary>
    /// Adds mounts to the collection.
    /// </summary>
    /// <param name="mounts">The mounts to add.</param>
    public void AddMounts(ImmutableArray<Mount> mounts)
    {
        Mounts = Mounts.SetItems(mounts.Select(m => new KeyValuePair<MountSymbol, Mount>(m.Symbol, m)));
    }
}
