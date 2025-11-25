using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Windows;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.GameModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.UI.Windows;
using System;
using System.Collections.Generic;

namespace SpaceTraders.UI;

public class RootScreen : ScreenObject, IDisposable
{
    private readonly ScreenSurface _mainSurface;
    public GameSession GameSession { get; init; }
    private bool disposed;
    private List<Window> _windows = [];
    private RootWindow _rootWindow;
    private GlyphSelectPopup _glyphWindow;

    private string? _mousePos { get; set; }

    public RootScreen(GameSession gameSession)
    {
#pragma warning disable S3010 // Static fields should not be updated in constructors
        Settings.ResizeMode = Settings.WindowResizeOptions.None;
#pragma warning restore S3010 // Static fields should not be updated in constructors

        // Create a surface that's the same size as the screen.
        _mainSurface = new ScreenSurface(
            Game.Instance.ScreenCellsX,
            Game.Instance.ScreenCellsY);
        GameSession = gameSession;

        Children.Add(_mainSurface);
        _rootWindow = new RootWindow(this);
        Children.Add(_rootWindow);
        _rootWindow.Show();

        SadConsole.Host.Game monoGameInstance = (SadConsole.Host.Game)SadConsole.Game.Instance.MonoGameInstance;
        monoGameInstance.WindowResized += MonoGameInstance_WindowResized;
        _glyphWindow = new SadConsole.UI.Windows.GlyphSelectPopup(19, 25, _mainSurface.Font, _mainSurface.FontSize);
        _rootWindow.Children.Add(_glyphWindow);
    }

    public virtual void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        _mainSurface.Dispose();
        _rootWindow.Dispose();
        _glyphWindow.Dispose();
        foreach (var window in _windows)
        {
            window.Dispose();
        }
    }

    protected virtual void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    private void MonoGameInstance_WindowResized(object? sender, EventArgs e)
    {
        var root = _mainSurface;
        var resizableSurface = (ICellSurfaceResize)root.Surface;

        resizableSurface.Resize(
            width: SadConsole.Settings.Rendering.RenderWidth / root.FontSize.X,
            height: SadConsole.Settings.Rendering.RenderHeight / root.FontSize.Y,
            clear: false);
        _rootWindow.ResizeAndRedraw(
            width: SadConsole.Settings.Rendering.RenderWidth / root.FontSize.X,
            height: SadConsole.Settings.Rendering.RenderHeight / root.FontSize.Y,
            clear: true);
    }

    internal void ShowWindow<TWindow, TData>(TData data) where TWindow : Window
    {
        var window = (TWindow?)Activator.CreateInstance(typeof(TWindow), data, this);
        if (window == null)
        {
            return;
        }
        _rootWindow.Children.Add(window);
        _windows.Add(window);
        window.Show();
    }

    internal void ShowAgentWindow()
    {
        ShowWindow<AgentWindow, Agent>(GameSession.Agent);
    }

    internal void ShowShipsWindow()
    {
        ShowWindow<ShipsWindow, Ship[]>(GameSession.Ships.ToArray());
    }

    internal void ShowGlyphWindow()
    {
        if (_glyphWindow.IsVisible)
            _glyphWindow.Hide();
        else
            _glyphWindow.Show();
    }

    internal void ShowShipWindow(Ship ship)
    {
        var shipWindow = new ShipWindow(ship, this);
        _windows.Add(shipWindow);
        _rootWindow.Children.Add(shipWindow);
        shipWindow.Show();
    }

    internal void HideAndDestroyWindow(Window window)
    {
        window.Hide();
        _rootWindow.Children.Remove(window);
        _windows.Remove(window);
        window.Dispose();
    }
}