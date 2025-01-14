using System.Diagnostics;

namespace YTEventBus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Check for Chrome processes
                    var chromeProcesses = Process.GetProcessesByName("chrome");
                    if (chromeProcesses.Length > 0)
                    {
                        _logger.LogInformation($"Chrome is running. Count: {chromeProcesses.Length}");
                    }
                    else
                    {
                        _logger.LogInformation("Chrome is not running.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking processes.");
                }
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
