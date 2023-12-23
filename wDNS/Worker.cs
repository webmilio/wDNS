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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Starting listener.");

#if SINGLETHREAD
        _listener.Listen(stoppingToken);
#else
        new Thread(() => _listener.Listen(stoppingToken)).Start();
#endif

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.ReadLine();
        }
    }
}