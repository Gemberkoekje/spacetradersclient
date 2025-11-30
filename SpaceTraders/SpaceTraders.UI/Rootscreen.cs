using Microsoft.Extensions.DependencyInjection;
using Qowaiv.Validation.Abstractions;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Windows;
using SpaceTraders.UI.Interfaces;
using SpaceTraders.UI.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

public class RootScreen : ScreenObject, IDisposable
{
    private readonly ControlsConsole _mainSurface;
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
        _mainSurface = new ControlsConsole(
            Game.Instance.ScreenCellsX,
            Game.Instance.ScreenCellsY);
        ServiceProvider = serviceProvider;

        Children.Add(_mainSurface);
        _rootWindow = new RootWindow(this);
        _mainSurface.Children.Add(_rootWindow);
        _rootWindow.Show();

        SadConsole.Host.Game monoGameInstance = (SadConsole.Host.Game)SadConsole.Game.Instance.MonoGameInstance;
        monoGameInstance.WindowResized += MonoGameInstance_WindowResized;
        _glyphWindow = new SadConsole.UI.Windows.GlyphSelectPopup(19, 25, _mainSurface.Font, _mainSurface.FontSize);
        _mainSurface.Children.Add(_glyphWindow);
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
    }

    internal void ShowWindow<TWindow>(string[] symbols) where TWindow : Window
    {
        var window = ServiceProvider.GetService<TWindow>();
        if (window == null)
        {
            return;
        }
        if (window is ICanSetSymbols dataWindow)
        {
            dataWindow.SetSymbol(symbols);
        }
        _mainSurface.Children.Add(window);
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
        _mainSurface.Children.Add(window);
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

    internal void HideAndDestroyWindow(Window window)
    {
        window.Hide();
        _mainSurface.Children.Remove(window);
        Windows.Remove(window);
        window.Dispose();
    }

    public void ScheduleCommand(Func<Task> command)
    {
        var backgroundUpdater = ServiceProvider.GetRequiredService<BackgroundDataUpdater>();
        backgroundUpdater?.ScheduleCommand(command);
    }
}