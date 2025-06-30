using System.Runtime.CompilerServices;

namespace wDNS.Extensions;

public static class LoggerExtensions
{
    public static void MethodCall<T>(this ILogger<T> logger, [CallerMemberName] string caller = "")   
    {
        logger.LogInformation("{CallerName} called on instance of {CallerType}", caller, typeof(T).FullName);
    }

    public static void InstancedWithProperties<T>(this ILogger logger, params (object property, object value)[] pairs)
    {
        logger.LogTrace("Instanciated object of type {Type}", typeof(T).FullName);

        if (pairs.Length == 0)
        {
            return;
        }

        foreach (var (property, value) in pairs)
        {
            logger.LogTrace("\t{Type}: {Name} = {Value}", typeof(T).Name, property, value);
        }
    }
}
