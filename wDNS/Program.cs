using wDNS.Common;
using wDNS.Forwarding;
using wDNS.Knowledge;
using wDNS.Knowledge.Caching;
using wDNS.Knowledge.HostFiles;
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

            .AddSingleton<IKnowledgeOrchestrator, KnowledgeOrchestrator>() // We don't tell it it's a IQuestionable since we don't want it to initialize itself.
            .AddSingleton<KnowledgeInitializer>()

            .AddSingleton<IAnswerCache, AnswerCache>()
            .AddSingleton<IQuestionable, AnswerCache>()
            .AddSingleton<IQuestionable, HostFilesStore>()

            .Configure<Configuration.Listening>(o => builder.Configuration.GetSection(nameof(Configuration.Listening)).Bind(o))
            .Configure<Configuration.Processing>(o => builder.Configuration.GetSection(nameof(Configuration.Processing)).Bind(o))
            .Configure<Configuration.Forwarding>(o => builder.Configuration.GetSection(nameof(Configuration.Forwarding)).Bind(o))
            .Configure<Configuration.SuppressWarnings>(o => builder.Configuration.GetSection(nameof(Configuration.SuppressWarnings)).Bind(o));

        var host = builder.Build();

        host.Run();
    }
}