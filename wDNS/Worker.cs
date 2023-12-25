using wDNS.Knowledge;
using wDNS.Listening;

namespace wDNS;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IListener _listener;
    private readonly KnowledgeOrchestrator _knowledge;

    public Worker(ILogger<Worker> logger, IListener listener, KnowledgeOrchestrator knowledge)
    {
        _logger = logger;
        _listener = listener;
        _knowledge = knowledge;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting listener");
        _logger.LogInformation("Configuring");

        await InitializeKnowledge();

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

    private Task InitializeKnowledge()
    {
        return _knowledge.Initialize();
    }
}