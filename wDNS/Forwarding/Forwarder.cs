using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using wDNS.Common;

namespace wDNS.Forwarding;

public class Forwarder : IForwarder, IDisposable
{
    private readonly ILogger<Forwarder> _logger;
    private readonly IOptions<Configuration.Forwarding> _config;

    private readonly UdpClient _udp;
    private readonly IPEndPoint[] _remotes;

    private bool disposedValue;

    public event Query.Delegate? Forwarding;
    public event Response.FromQuestionDelegate? Received;

    public Forwarder(ILogger<Forwarder> logger, IOptions<Configuration.Forwarding> config)
    {
        _logger = logger;
        _config = config;

        _udp = new UdpClient(_config.Value.Port);
        _remotes = _config.Value.GetRemotes();
    }

    public async Task<Response> ForwardAsync(Query query, CancellationToken stoppingToken)
    {
        int ptr = 0;
        var buffer = new byte[255];

        query.Write(buffer, ref ptr);
        Array.Resize(ref buffer, ptr);

        var remote = _remotes[0]; // TODO Change this to use multiple servers.
        _logger.LogDebug("Forwarding request {{{Questions}}} to {Remote}.", string.Join(',', query.Questions), remote);

        Forwarding?.Invoke(this, query);

        await _udp.SendAsync(buffer, remote, stoppingToken);
        var received = await _udp.ReceiveAsync(stoppingToken);

        ptr = 0;
        var response = Response.Read(received.Buffer, ref ptr);

        Received?.Invoke(this, query, response);

        _logger.LogDebug("Received forwarded request {{{Questions}}} response from {Remote}: {{{Answers}}}", 
            string.Join(',', query.Questions), remote, string.Join(',', response.Answers));

        return response;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _udp.Dispose();
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
