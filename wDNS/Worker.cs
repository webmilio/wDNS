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
        _logger.LogInformation("Starting listener.");

#if SINGLE_THREAD
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