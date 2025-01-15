using YTEventBus;

var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "YTEventBusService";
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders() 
        .AddEventLog() // Add EventLog provider (only for windos)
        .AddConsole() // Add Console provider (for debugging)
        .SetMinimumLevel(LogLevel.Information);
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    });

var host = builder.Build();
host.Run();
