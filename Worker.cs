using Microsoft.Extensions.Logging;

namespace Development;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly UsbWatcher _watcher;

    public Worker(ILogger<Worker> logger, UsbWatcher watcher)
    {
        _logger = logger;
        _watcher = watcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _watcher.Start();

        while(!stoppingToken.IsCancellationRequested) {
            _logger.LogDebug("Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(1_000, stoppingToken);
        }

        _watcher.Stop();
    }
}
