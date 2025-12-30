// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SadConsole;
using SadConsole.Configuration;
using SpaceTraders.Client;
using SpaceTraders.Core.Services;
using SpaceTraders.UI;
using SpaceTraders.UI.Windows;
using System;
using System.Net.Http.Headers;

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
        services.AddSingleton<RootScreen>((s) => new RootScreen(s));
        services.AddSingleton<AgentService>();
        services.AddSingleton<ShipService>();
        services.AddSingleton<ShipNavService>();
        services.AddSingleton<SystemService>();
        services.AddSingleton<SpaceTradersService>();
        services.AddSingleton<ContractService>();
        services.AddSingleton<WaypointService>();
        services.AddSingleton<ShipyardService>();
        services.AddSingleton<ModuleService>();
        services.AddSingleton<MarketService>();

        services.AddTransient<AgentWindow>();
        services.AddTransient<ContractWindow>();
        services.AddTransient<NavigationWindow>();
        services.AddTransient<RootWindow>();
        services.AddTransient<ShipsWindow>();
        services.AddTransient<ShipWindow>();
        services.AddTransient<SystemDataWindow>();
        services.AddTransient<SystemMapWindow>();
        services.AddTransient<WarningWindow>();
        services.AddTransient<WaypointWindow>();
        services.AddTransient<StarMapWindow>();
        services.AddTransient<CargoWindow>();
        services.AddTransient<CrewWindow>();
        services.AddTransient<EngineWindow>();
        services.AddTransient<FrameWindow>();
        services.AddTransient<ModulesWindow>();
        services.AddTransient<ModuleWindow>();
        services.AddTransient<MountsWindow>();
        services.AddTransient<MountWindow>();
        services.AddTransient<ReactorWindow>();
        services.AddTransient<ShipyardWindow>();
        services.AddTransient<TransactionsWindow>();
        services.AddTransient<ShipyardShipWindow>();
        services.AddTransient<SystemsWindow>();
        services.AddTransient<ShipyardModulesWindow>();
        services.AddTransient<ShipyardMountsWindow>();
        services.AddTransient<MarketWindow>();
        services.AddTransient<RouteWindow>();

        services.AddSingleton<BackgroundDataUpdater>();
        services.AddSingleton<Scheduler>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<BackgroundDataUpdater>());
    })
    .Build();

Settings.WindowTitle = "Space Traders";

SadConsole.Configuration.Builder
    .GetBuilder()
    .SetWindowSizeInCells(90, 50)
    .SetStartingScreen((_) => host.Services.GetRequiredService<RootScreen>())
    .IsStartingScreenFocused(true)
    .ConfigureFonts(true)
    .OnStart((_, _) =>
    {
        host.StartAsync().GetAwaiter().GetResult();
    })
    .OnEnd((_, _) =>
    {
        host.StopAsync().GetAwaiter().GetResult();
    }).Run();
