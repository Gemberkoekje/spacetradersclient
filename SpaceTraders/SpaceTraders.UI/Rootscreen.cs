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

/// <summary>
/// The root screen for the SpaceTraders UI application.
/// </summary>
public sealed class RootScreen : ScreenObject, IDisposable
{
    private readonly ControlsConsole _mainSurface;

    private IServiceProvider ServiceProvider { get; init; }

    private bool _disposed;

    /// <summary>
    /// Gets the list of open windows.
    /// </summary>
    public List<Window> Windows { get; } = [];

    private readonly RootWindow _rootWindow;

    private readonly GlyphSelectPopup _glyphWindow;

    private Dictionary<string, (CustomLabel Label, (int X, int Y) Location)> RightJustifiedBinds { get; init; } = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="RootScreen"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
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

    /// <summary>
    /// Loads data for the logged-in agent.
    /// </summary>
    /// <param name="data">The agent data.</param>
    public void LoadData(Agent? data)
    {
        RightJustifiedBinds["LoggedInAs"].Label.SetData([$"{data?.Symbol}"]);
        RightJustifiedBinds["Credits"].Label.SetData([$"{data?.Credits:#,###}"]);
        UpdateRightJustifiableBinds();
    }

    /// <summary>
    /// Updates the clock display.
    /// </summary>
    /// <param name="dateTime">The current date and time.</param>
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

    /// <summary>
    /// Disposes of the root screen and all its resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _mainSurface.Dispose();
        _rootWindow.Dispose();
        _glyphWindow.Dispose();
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
    }

    /// <summary>
    /// Schedules a command to be executed by the background updater.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    public void ScheduleCommand(Func<Task> command)
    {
        var backgroundUpdater = ServiceProvider.GetRequiredService<BackgroundDataUpdater>();
        backgroundUpdater?.ScheduleCommand(command);
    }
}
