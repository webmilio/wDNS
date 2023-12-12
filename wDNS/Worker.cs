using System.Net;
using System.Net.Sockets;
using wDNS.Caching;
using wDNS.Listening;
using wDNS.Models;

namespace wDNS;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IRequestListener _listener;

    private Cache<Question, Answer> _cache = new();
    private volatile object _cacheLock = new();

    public Worker(ILogger<Worker> logger, IRequestListener listener)
    {
        _logger = logger;
        _listener = listener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(() => _listener.Listen(stoppingToken)).Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            // This will likely be used for various misc. operations.
            await Task.Delay(1000, stoppingToken);
        }
    }
}