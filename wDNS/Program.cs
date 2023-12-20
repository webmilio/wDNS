using wDNS.Caching;
using wDNS.Forwarding;
using wDNS.Listening;
using wDNS.Processing;

namespace wDNS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
            .AddHostedService<Worker>()
            .AddSingleton<IListener, Listener>()
            .AddSingleton<IProcessor, Processor>()
            .AddSingleton<IForwarder, Forwarder>()

            .AddSingleton<IAnswerCache, AnswerCache>()
            
            .Configure<Configuration.Listening>(o => builder.Configuration.GetSection(nameof(Configuration.Listening)).Bind(o))
            .Configure<Configuration.Forwarding>(o => builder.Configuration.GetSection(nameof(Configuration.Forwarding)).Bind(o))
            .Configure<Configuration.SuppressWarnings>(o => builder.Configuration.GetSection(nameof(Configuration.SuppressWarnings)).Bind(o));

        var host = builder.Build();

        host.Run();
    }
}