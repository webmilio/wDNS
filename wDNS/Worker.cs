using System.Reflection;
using wDNS.Knowledge;
using wDNS.Listening;

namespace wDNS;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IListener _listener;

    private readonly HostFileReader _hosts;

    public Worker(ILogger<Worker> logger, IListener listener, HostFileReader hosts)
    {
        _logger = logger;
        _listener = listener;

        _hosts = hosts;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Starting listener.");
        _logger.LogInformation("Configuring");

        await ReadConfigurations();

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

    private async Task ReadConfigurations()
    {
        _logger.LogInformation("Reading {{conf}} directory");

#if DEBUG
        Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

        var confDir = new DirectoryInfo("conf");
        confDir.Create();

        _logger.LogInformation("Reading Hosts files");
        var hostsDir = confDir.CreateSubdirectory("hosts");

        await _hosts.Read(hostsDir);
    }
}