using YTEventBus;

var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "YTEventBusService";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders() 
        .AddEventLog() // Add EventLog provider (inly for windos)
        .AddConsole() // Add Console provider (for debugging)
        .SetMinimumLevel(LogLevel.Information);
    });

var host = builder.Build();
host.Run();
