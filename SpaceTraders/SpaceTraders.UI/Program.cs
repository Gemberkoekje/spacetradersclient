// See https://aka.ms/new-console-template for more information
using DotNetProjectFile.MsBuild;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SadConsole;
using SadConsole.Configuration;
using Spacetraders.Client;
using SpaceTraders.Client;
using SpaceTraders.Core.Loaders;
using SpaceTraders.Core.Models.GameModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI;
using System;
using System.Net.Http.Headers;
using static SadConsole.Settings;

// Generic host setup (gives config, logging, DI).
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddHttpClient<SpaceTradersClient>(client =>
        {
            var token = ctx.Configuration["SpaceTraders:Token"];
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Missing SpaceTraders token (SpaceTraders:Token). Configure user secrets or env var.");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        });
        // RootScreen currently has a parameterless ctor, register for potential future DI.
        services.AddTransient<RootScreen>();
        services.AddTransient<SplashScreen>();
        services.AddTransient<AgentService>();
        services.AddTransient<ShipService>();
        services.AddTransient<SystemService>();
        services.AddSingleton<GameSession>();
        services.AddSingleton<SpaceTradersService>();
    })
    .Build();

Settings.WindowTitle = "Space Traders";

SadConsole.Configuration.Builder
    .GetBuilder()
    .SetWindowSizeInCells(90, 50)
    .SetStartingScreen((_) => host.Services.GetRequiredService<RootScreen>())
    .IsStartingScreenFocused(true)
    .ConfigureFonts(true)
    .Run();
