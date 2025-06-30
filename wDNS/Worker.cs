using wDNS.Listening;

namespace wDNS;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IListener _listener;

    public Worker(ILogger<Worker> logger, IListener listener)
    {
        _logger = logger;
        _listener = listener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _listener.StartAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
