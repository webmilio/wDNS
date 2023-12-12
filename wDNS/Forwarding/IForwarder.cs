using wDNS.Models;

namespace wDNS.Forwarding;

public interface IForwarder
{
    public Task<Response> ForwardAsync(Query query, CancellationToken stoppingToken);
}
