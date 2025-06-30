using wDNS.Common.Models;
using wDNS.Conversion.Readable;
using wDNS.Extensions;
using wDNS.Forwarding;
using wDNS.Listening;
using wDNS.Processing;

namespace wDNS;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Services
            .AddHostedService<Worker>()
            .AddMemoryCache();

        RegisterReadableConverters(builder.Services);
        RegisterServices(builder.Services);

        Configure(builder);

        var host = builder.Build();
        return host.RunAsync();
    }

    private static void RegisterReadableConverters(IServiceCollection services)
    {
        services
            .AddSingleton<IReadableStringConverter<Answer>, AnswerConverter>()
            .AddSingleton<IReadableStringConverter<LabelSegment>, LabelSegmentConverter>()
            .AddSingleton<IReadableStringConverter<Label>, LabelConverter>()
            .AddSingleton<IReadableStringConverter<Question>, QuestionConverter>()
            .AddSingleton<IReadableStringConverter<Message>, MessageConverter>()
            .AddSingleton<IReadableStringConverter<Response>, ResponseConverter>()
            .AddSingleton<IReadableStringConverter<Request>, RequestConverter>();
    }
    
    private static void RegisterServices(IServiceCollection services)
    {
        services
            .AddSingleton<IForwarder, RemoteForwarder>();
    }

    private static void Configure(IHostApplicationBuilder builder)
    {
        builder
            .ConfigureReplacable<Configuration.Listening, IListener, UdpListener>()
            .ConfigureReplacable<Configuration.Processing, IRequestProcessor, RequestProcess>();

        builder.Services
            .Configure<Configuration.Processing>(o => builder.Configuration.GetSection(nameof(Configuration.Processing)).Bind(o))
            .Configure<Configuration.Forwarding>(o => builder.Configuration.GetSection(nameof(Configuration.Forwarding)).Bind(o))
            .Configure<Configuration.SuppressWarnings>(o => builder.Configuration.GetSection(nameof(Configuration.SuppressWarnings)).Bind(o));
    }
}