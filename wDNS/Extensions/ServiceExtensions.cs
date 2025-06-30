using wDNS.Configuration;

namespace wDNS.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterSingletonChildren<TParent>(this IServiceCollection services)
    {
        return services;
    }

    public static IHostApplicationBuilder ConfigureReplacable<TConfiguration, TServiceType, TDefaultServiceType>(this IHostApplicationBuilder builder)
        where TConfiguration : ComponentConfiguration
    {
        var section = typeof(TConfiguration).Name;

        builder.Services.Configure<TConfiguration>(o => builder.Configuration.GetSection(section).Bind(o));
        var instance = builder.Configuration.GetSection(section).Get<TConfiguration>();

        Type? serviceType;

        if (instance == null || string.IsNullOrEmpty(instance.ServiceType))
        {
            serviceType = typeof(TDefaultServiceType);
        }
        else
        {
            serviceType = Type.GetType(instance.ServiceType);
        }

        const string Error = "Error while registering replacable service: attempted to bind service type {0} to configured type {1}";

        if (serviceType == null)
        {
            throw new InvalidOperationException(string.Format(Error, typeof(TServiceType), instance!.ServiceType));
        }

        if (serviceType.IsAbstract || serviceType.IsInterface)
        {
            throw new InvalidOperationException($"{string.Format(Error, typeof(TServiceType), instance!.ServiceType)} but type is abstract or interface.");
        }

        builder.Services.AddSingleton(typeof(TServiceType), serviceType);
        return builder;
    }
}
