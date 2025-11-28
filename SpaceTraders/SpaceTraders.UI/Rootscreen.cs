using Microsoft.Extensions.DependencyInjection;
using Qowaiv.Validation.Abstractions;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Windows;
using SpaceTraders.Core.Loaders;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Models.ContractModels;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.Interfaces;
using SpaceTraders.UI.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

public class RootScreen : ScreenObject, IDisposable
{
    private readonly ScreenSurface _mainSurface;
    IServiceProvider ServiceProvider { get; init; }
    private bool disposed;
    public List<Window> Windows = [];
    private RootWindow _rootWindow;
    private GlyphSelectPopup _glyphWindow;

    private string? _mousePos { get; set; }

    public RootScreen(IServiceProvider serviceProvider)
    {
#pragma warning disable S3010 // Static fields should not be updated in constructors
        Settings.ResizeMode = Settings.WindowResizeOptions.None;
#pragma warning restore S3010 // Static fields should not be updated in constructors

        // Create a surface that's the same size as the screen.
        _mainSurface = new ScreenSurface(
            Game.Instance.ScreenCellsX,
            Game.Instance.ScreenCellsY);
        ServiceProvider = serviceProvider;

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
        foreach (var window in Windows)
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

    internal void ShowWindow<TWindow>(string? symbol = null, string? parentsymbol = null) where TWindow : Window
    {
        var window = ServiceProvider.GetService<TWindow>();
        if (window == null)
        {
            return;
        }
        if (window is ICanLoadData dataWindow)
        {
            dataWindow.Symbol = symbol;
            dataWindow.ParentSymbol = parentsymbol;
        }
        _rootWindow.Children.Add(window);
        Windows.Add(window);
        window.Show();
    }

    internal void ShowWarningWindow(Result result)
    {
        var window = ServiceProvider.GetService<WarningWindow>();
        if (window == null)
        {
            return;
        }
        window.LoadData(result);
        _rootWindow.Children.Add(window);
        Windows.Add(window);
        window.Show();
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
        var window = ServiceProvider.GetService<ShipWindow>();
        if (window == null)
        {
            return;
        }
        window.LoadData(ship);
        Windows.Add(window);
        _rootWindow.Children.Add(window);
        window.Show();
    }

    internal void HideAndDestroyWindow(Window window)
    {
        window.Hide();
        _rootWindow.Children.Remove(window);
        Windows.Remove(window);
        window.Dispose();
    }

    public void DoAsynchronousEventually(Func<Task> action)
    {
        var backgroundUpdater = ServiceProvider.GetRequiredService<BackgroundDataUpdater>();
        backgroundUpdater?.DoAsynchronousEventually(action);
    }
}