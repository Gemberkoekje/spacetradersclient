using SpaceTraders.Core.Enums;
using SpaceTraders.Core.Models.ShipModels;
using System.Collections.Immutable;

namespace SpaceTraders.Core.Services;

public sealed class ModuleService
{
    private ImmutableDictionary<FrameSymbol, Frame> Frames { get; set; } = [];

    private ImmutableDictionary<ReactorSymbol, Reactor> Reactors { get; set; } = [];

    private ImmutableDictionary<EngineSymbol, Engine> Engines { get; set; } = [];

    private ImmutableDictionary<ModuleSymbol, Module> Modules { get; set; } = [];

    private ImmutableDictionary<MountSymbol, Mount> Mounts { get; set; } = [];

    public ImmutableDictionary<FrameSymbol, Frame> GetFrames()
    {
        return Frames;
    }

    public ImmutableDictionary<ReactorSymbol, Reactor> GetReactors()
    {
        return Reactors;
    }

    public ImmutableDictionary<EngineSymbol, Engine> GetEngines()
    {
        return Engines;
    }

    public ImmutableDictionary<ModuleSymbol, Module> GetModules()
    {
        return Modules;
    }

    public ImmutableDictionary<MountSymbol, Mount> GetMounts()
    {
        return Mounts;
    }

    public void AddFrames(ImmutableArray<Frame> frames)
    {
        foreach (var frame in frames)
        {
            Frames = Frames.SetItem(frame.Symbol, frame);
        }
    }

    public void AddReactors(ImmutableArray<Reactor> reactors)
    {
        foreach (var reactor in reactors)
        {
            Reactors = Reactors.SetItem(reactor.Symbol, reactor);
        }
    }

    public void AddEngines(ImmutableArray<Engine> engines)
    {
        foreach (var engine in engines)
        {
            Engines = Engines.SetItem(engine.Symbol, engine);
        }
    }

    public void AddModules(ImmutableArray<Module> modules)
    {
        foreach (var module in modules)
        {
            Modules = Modules.SetItem(module.Symbol, module);
        }
    }

    public void AddMounts(ImmutableArray<Mount> mounts)
    {
        foreach (var mount in mounts)
        {
            Mounts = Mounts.SetItem(mount.Symbol, mount);
        }
    }
}
