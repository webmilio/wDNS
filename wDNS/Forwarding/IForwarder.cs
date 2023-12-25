using wDNS.Common.Models;

namespace wDNS.Forwarding;

public interface IForwarder
{
    public Task<Response> ForwardAsync(Request query, CancellationToken stoppingToken);
}
