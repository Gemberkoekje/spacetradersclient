using Microsoft.Extensions.DependencyInjection;
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
    private List<Window> _windows = [];
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
        var window = ServiceProvider.GetService<TWindow>();
        if (window == null)
        {
            return;
        }
        if (window is ICanLoadData<TData> dataWindow)
        {
            dataWindow.LoadData(data);
        }
        _rootWindow.Children.Add(window);
        _windows.Add(window);
        window.Show();
    }

    internal async Task ShowAgentWindow()
    {
        var agentService = ServiceProvider.GetRequiredService<AgentService>();
        ShowWindow<AgentWindow, Agent>(await agentService.GetAgent());
    }

    internal async Task ShowShipsWindow()
    {
        var shipService = ServiceProvider.GetRequiredService<ShipService>();
        ShowWindow<ShipsWindow, Ship[]>(await shipService.GetMyShips());
    }

    internal async Task ShowContractWindow()
    {
        var contractService = ServiceProvider.GetRequiredService<ContractService>();
        var contracts = await contractService.GetMyContracts();
        ShowWindow<ContractWindow, Contract?>(contracts.Length > 0 ? contracts[0] : null);
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
        _windows.Add(window);
        _rootWindow.Children.Add(window);
        window.Show();
    }

    internal void HideAndDestroyWindow(Window window)
    {
        window.Hide();
        _rootWindow.Children.Remove(window);
        _windows.Remove(window);
        window.Dispose();
    }
}