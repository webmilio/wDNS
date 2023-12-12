using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net;
using wDNS.Processing;

namespace wDNS.Listening;

public class RequestListener : IRequestListener, IDisposable
{
    private readonly ILogger<RequestListener> _logger;
    private readonly IOptions<Configuration.Listening> _config;
    private readonly IOptions<Configuration.SuppressWarnings> _suppressed;

    private readonly IRequestProcessor _processor;

    private readonly UdpClient _udp;
    private bool disposedValue;

    public RequestListener(ILogger<RequestListener> logger, 
        IOptions<Configuration.Listening> config, IOptions<Configuration.SuppressWarnings> suppressed,
        IRequestProcessor processor)
    {
        _logger = logger;
        _config = config;
        _suppressed = suppressed;

        _processor = processor;

        _udp = new UdpClient(_config.Value.Port);
    }

    public void Listen(CancellationToken stoppingToken)
    {
        var ip = _config.Value._IPAddress;

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Receiving requests...");

            var local = new IPEndPoint(ip, _config.Value.Port);
            byte[] received;

            try
            {
                received = _udp.Receive(ref local);
            }
            catch (SocketException e)
            {
                if (!_suppressed.Value.UDPConnectionReset)
                {
                    _logger.LogWarning("Socket exception: {Exception}", e);
                }

                continue;
            }

            _logger.LogDebug("Processing DNS request from {RemoteEndPoint}", local);
            Task.Run(async () => await _processor.ProcessAsync(_udp, new UdpReceiveResult(received, local), stoppingToken), stoppingToken);
        }
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

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Listener()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
