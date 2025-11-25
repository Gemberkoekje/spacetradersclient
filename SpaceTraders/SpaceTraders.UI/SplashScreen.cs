using Microsoft.Extensions.DependencyInjection;
using SadConsole;
using SpaceTraders.Core.Models.GameModels;
using System;
using System.Threading;

namespace SpaceTraders.UI;

public sealed class SplashScreen : ScreenSurface
{
    private readonly GameSession _session;
    private readonly IServiceProvider _services;
    private bool _started;
    private readonly CancellationTokenSource _cts = new();

    private double _lastProgressDrawn = -1;

    public SplashScreen(GameSession session, IServiceProvider services)
        : base(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY)
    {
        _session = session;
        _services = services;

    }

    public override void Update(TimeSpan time)
    {
        base.Update(time);

        if (!_started)
        {
            _started = true;
            _session.BeginInitialization(_cts.Token);
        }

        // Redraw only when progress changes noticeably.
        if (Math.Abs(_session.Progress - _lastProgressDrawn) > 0.01)
        {
            _lastProgressDrawn = _session.Progress;
            DrawProgress();
        }

        if (_session.IsInitialized)
        {
            // Switch to real starting/root screen resolved from DI.
            var root = _services.GetRequiredService<RootScreen>();
            GameHost.Instance.Screen = root; // Transfer control.
            IsVisible = false;
            _cts.Dispose();
        }
        else if (_session.IsFailed)
        {
            DrawError(_session.InitializationError);
        }
    }

    private void DrawProgress()
    {
        var console = this.Surface;
        console.Clear();
        var pct = (int)(_session.Progress * 100);
        var barWidth = console.Width - 10;
        var fill = (int)(barWidth * _session.Progress);
        var y = console.Height / 2;

        console.Print(0, y - 2, "Loading SpaceTraders data...");
        console.Print(0, y, "[" + new string('#', fill) + new string('.', barWidth - fill) + $"] {pct}%");
        console.Print(0, y + 2, "Agent, Ships");
    }

    private void DrawError(Exception? ex)
    {
        var console = this.Surface;
        console.Clear();
        console.Print(0, console.Height / 2 - 1, "Initialization FAILED");
        console.Print(0, console.Height / 2, ex?.GetBaseException().Message ?? "Unknown error");
        console.Print(0, console.Height / 2 + 2, "Press ESC to exit.");
    }
}