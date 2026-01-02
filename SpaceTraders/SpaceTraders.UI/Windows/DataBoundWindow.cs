using SpaceTraders.Core.IDs;
using SpaceTraders.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

/// <summary>
/// Base class for windows that display data from a service and react to updates.
/// Provides automatic event subscription cleanup on disposal.
/// </summary>
/// <typeparam name="TData">The type of data this window displays.</typeparam>
public abstract class DataBoundWindow<TData> : ClosableWindow
{
    private readonly List<Action> _unsubscribeActions = [];
    private bool _disposed;
    private bool _initialized;

    /// <summary>
    /// The currently displayed data. Used for change detection.
    /// </summary>
    protected TData? CurrentData { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBoundWindow{TData}"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="width">The initial width.</param>
    /// <param name="height">The initial height.</param>
    protected DataBoundWindow(RootScreen rootScreen, int width, int height)
        : base(rootScreen, width, height)
    {
    }

    /// <summary>
    /// Initializes the window by drawing content. Call this at the end of derived class constructors.
    /// For windows without symbols, also performs initial data refresh.
    /// </summary>
    /// <param name="refreshImmediately">Whether to refresh data immediately.</param>
    protected void Initialize(bool refreshImmediately = false)
    {
        if (_initialized) return;
        _initialized = true;

        DrawContent();
        if (refreshImmediately)
        {
            RefreshData();
        }
    }

    /// <summary>
    /// Subscribes to a service event and tracks it for automatic cleanup on disposal.
    /// </summary>
    /// <typeparam name="TEvent">The event argument type.</typeparam>
    /// <param name="subscribe">Action to subscribe (e.g., service.Updated += handler).</param>
    /// <param name="unsubscribe">Action to unsubscribe (e.g., service.Updated -= handler).</param>
    /// <param name="handler">The event handler.</param>
    protected void SubscribeToEvent<TEvent>(
        Action<Func<TEvent, Task>> subscribe,
        Action<Func<TEvent, Task>> unsubscribe,
        Func<TEvent, Task> handler)
    {
        subscribe(handler);
        _unsubscribeActions.Add(() => unsubscribe(handler));
    }

    /// <summary>
    /// Subscribes to a synchronous service event and tracks it for automatic cleanup.
    /// </summary>
    /// <typeparam name="TEvent">The event argument type.</typeparam>
    /// <param name="subscribe">Action to subscribe.</param>
    /// <param name="unsubscribe">Action to unsubscribe.</param>
    /// <param name="handler">The event handler.</param>
    protected void SubscribeToEvent<TEvent>(
        Action<Action<TEvent>> subscribe,
        Action<Action<TEvent>> unsubscribe,
        Action<TEvent> handler)
    {
        subscribe(handler);
        _unsubscribeActions.Add(() => unsubscribe(handler));
    }

    /// <summary>
    /// Override to perform additional setup when context/symbols are set, before data is fetched.
    /// </summary>
    protected virtual void OnSymbolsSet() { }

    /// <summary>
    /// Called to initialize the window's controls and bind dictionary.
    /// Called once during construction.
    /// </summary>
    protected abstract void DrawContent();

    /// <summary>
    /// Fetches the current data from the service(s).
    /// Return null if data is not yet available.
    /// </summary>
    /// <returns>The fetched data, or null if not available.</returns>
    protected abstract TData? FetchData();

    /// <summary>
    /// Compares current data with previous data to determine if a refresh is needed.
    /// Default implementation uses reference equality.
    /// </summary>
    /// <param name="current">The current data.</param>
    /// <param name="previous">The previous data.</param>
    /// <returns>True if the data is equal, false otherwise.</returns>
    protected virtual bool DataEquals(TData? current, TData? previous)
    {
        if (current is null && previous is null) return true;
        if (current is null || previous is null) return false;
        return ReferenceEquals(current, previous) || current.Equals(previous);
    }

    /// <summary>
    /// Updates the UI bindings with the new data.
    /// Called when data has changed.
    /// </summary>
    /// <param name="data">The data to bind.</param>
    protected abstract void BindData(TData data);

    /// <summary>
    /// Refreshes the window by fetching and binding data if it has changed.
    /// </summary>
    protected void RefreshData()
    {
        if (Surface == null) return;

        var newData = FetchData();
        if (newData is null) return;
        if (CurrentData is not null && DataEquals(newData, CurrentData)) return;

        CurrentData = newData;
        BindData(newData);
        ResizeAndRedraw();
    }

    /// <summary>
    /// Handler for service events that triggers a data refresh.
    /// Use this with SubscribeToEvent for async events.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="_">The event data (unused).</param>
    /// <returns>A completed task.</returns>
    protected Task OnServiceUpdated<TEvent>(TEvent _)
    {
        RefreshData();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handler for service events that triggers a data refresh.
    /// Use this with SubscribeToEvent for sync events.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="_">The event data (unused).</param>
    protected void OnServiceUpdatedSync<TEvent>(TEvent _)
    {
        RefreshData();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe from all tracked events
                foreach (var unsubscribe in _unsubscribeActions)
                {
                    try
                    {
                        unsubscribe();
                    }
                    catch
                    {
                        // Ignore errors during cleanup
                    }
                }
                _unsubscribeActions.Clear();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}

/// <summary>
/// Base class for windows that don't require context parameters but still display service data.
/// Derived classes must call Initialize(refreshImmediately: true) at the end of their constructor.
/// </summary>
/// <typeparam name="TData">The type of data this window displays.</typeparam>
public abstract class DataBoundWindowNoSymbols<TData> : DataBoundWindow<TData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataBoundWindowNoSymbols{TData}"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="width">The initial width.</param>
    /// <param name="height">The initial height.</param>
    protected DataBoundWindowNoSymbols(RootScreen rootScreen, int width, int height)
        : base(rootScreen, width, height)
    {
        // Derived classes must call Initialize(refreshImmediately: true) at end of their constructor
    }
}

/// <summary>
/// Non-generic interface for context-based windows to enable simpler ShowWindow calls.
/// </summary>
/// <typeparam name="TContext">The symbol context type.</typeparam>
public interface IDataBoundWindowWithContext<in TContext>
    where TContext : ISymbolContext
{
    /// <summary>
    /// Sets the context for the window.
    /// </summary>
    /// <param name="context">The context to set.</param>
    void SetContext(TContext context);
}

/// <summary>
/// Base class for windows that require a typed symbol context.
/// Derived classes must call Initialize() at the end of their constructor.
/// </summary>
/// <typeparam name="TData">The type of data this window displays.</typeparam>
/// <typeparam name="TContext">The symbol context type containing all required symbols.</typeparam>
public abstract class DataBoundWindowWithContext<TData, TContext> : DataBoundWindow<TData>, IDataBoundWindowWithContext<TContext>
    where TContext : ISymbolContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataBoundWindowWithContext{TData, TContext}"/> class.
    /// </summary>
    /// <param name="rootScreen">The root screen.</param>
    /// <param name="width">The initial width.</param>
    /// <param name="height">The initial height.</param>
    protected DataBoundWindowWithContext(RootScreen rootScreen, int width, int height)
        : base(rootScreen, width, height)
    {
        // Derived classes must call Initialize() at end of their constructor
    }

    /// <summary>
    /// Gets the symbol context containing all required symbols for this window.
    /// </summary>
    protected TContext Context { get; private set; } = default!;

    /// <summary>
    /// Called by RootScreen when the window is shown with a context.
    /// </summary>
    /// <param name="context">The context to set.</param>
    public void SetContext(TContext context)
    {
        Context = context;
        OnSymbolsSet();
        RefreshData();
    }
}
