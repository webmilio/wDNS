using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using wDNS.Models;

namespace wDNS.Forwarding;

public class Forwarder : IForwarder, IDisposable
{
    private readonly ILogger<Forwarder> _logger;
    private readonly IOptions<Configuration.Forwarding> _config;

    private readonly UdpClient _udp;
    private readonly IPEndPoint[] _remotes;

    private bool disposedValue;

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
        await _udp.SendAsync(buffer, remote, stoppingToken);

        var received = await _udp.ReceiveAsync(stoppingToken);
        
        ptr = 0;
        var response = Response.Read(received.Buffer, ref ptr);

        return response;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _udp.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
