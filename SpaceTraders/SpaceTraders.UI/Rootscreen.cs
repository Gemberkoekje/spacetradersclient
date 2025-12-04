using Microsoft.Extensions.DependencyInjection;
using Qowaiv.Validation.Abstractions;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Windows;
using SpaceTraders.Core.Models.AgentModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
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
    protected Dictionary<string, (CustomLabel Label, (int X, int Y) Location)> RightJustifiedBinds { get; init; } = new();

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
        RightJustifiedBinds.Add("TimeLabel", (_mainSurface.Controls.AddLabel("Current UTC Time:", Game.Instance.ScreenCellsX - 20, 1), (20, 1)));
        RightJustifiedBinds.Add("Time", (_mainSurface.Controls.AddLabel("[Time]", Game.Instance.ScreenCellsX - 2, 1), (2, 1)));
        RightJustifiedBinds.Add("LoggedInAsLabel", (_mainSurface.Controls.AddLabel("Logged in as:", Game.Instance.ScreenCellsX - 20, 2), (20, 2)));
        RightJustifiedBinds.Add("LoggedInAs", (_mainSurface.Controls.AddLabel("[name]", Game.Instance.ScreenCellsX - 2, 2), (2, 2)));
        RightJustifiedBinds.Add("CreditsLabel", (_mainSurface.Controls.AddLabel("Credits:", Game.Instance.ScreenCellsX - 20, 3), (20, 3)));
        RightJustifiedBinds.Add("Credits", (_mainSurface.Controls.AddLabel("[Credits]", Game.Instance.ScreenCellsX - 2, 3), (2, 3)));
        foreach (var bind in RightJustifiedBinds.Values)
        {
            bind.Label.Position = (Game.Instance.ScreenCellsX - (bind.Location.X + bind.Label.ActualWidth), bind.Location.Y);
        }
        _rootWindow = new RootWindow(this);
        _mainSurface.Children.Add(_rootWindow);
        _rootWindow.Show();

        SadConsole.Host.Game monoGameInstance = (SadConsole.Host.Game)SadConsole.Game.Instance.MonoGameInstance;
        monoGameInstance.WindowResized += MonoGameInstance_WindowResized;
        _glyphWindow = new SadConsole.UI.Windows.GlyphSelectPopup(19, 25, _mainSurface.Font, _mainSurface.FontSize);
        _mainSurface.Children.Add(_glyphWindow);
        ServiceProvider.GetRequiredService<AgentService>().Updated += LoadData;
    }

    public void LoadData(Agent? data)
    {
        RightJustifiedBinds["LoggedInAs"].Label.SetData([$"{data.Symbol}"]);
        RightJustifiedBinds["Credits"].Label.SetData([$"{data.Credits:#,###}"]);
        UpdateRightJustifiableBinds();
    }

    public void UpdateClock(DateTime dateTime)
    {
        RightJustifiedBinds["Time"].Label.SetData([$"{dateTime: d-M-yyyy HH:mm:ss}"]);
        UpdateRightJustifiableBinds();
    }

    private void UpdateRightJustifiableBinds()
    {
        var maxJustifiableJustify = Math.Max(RightJustifiedBinds["LoggedInAs"].Label.ActualWidth, Math.Max(RightJustifiedBinds["Credits"].Label.ActualWidth, RightJustifiedBinds["Time"].Label.ActualWidth));
        RightJustifiedBinds["TimeLabel"] = (RightJustifiedBinds["TimeLabel"].Label, (maxJustifiableJustify + 3, RightJustifiedBinds["TimeLabel"].Location.Y));
        RightJustifiedBinds["LoggedInAsLabel"] = (RightJustifiedBinds["LoggedInAsLabel"].Label, (maxJustifiableJustify + 3, RightJustifiedBinds["LoggedInAsLabel"].Location.Y));
        RightJustifiedBinds["CreditsLabel"] = (RightJustifiedBinds["CreditsLabel"].Label, (maxJustifiableJustify + 3, RightJustifiedBinds["CreditsLabel"].Location.Y));
        MonoGameInstance_WindowResized(null, EventArgs.Empty);
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

        foreach (var bind in RightJustifiedBinds.Values)
        {
            bind.Label.Position = (root.Width - (bind.Location.X + bind.Label.ActualWidth), bind.Location.Y);
        }
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