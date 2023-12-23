using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net;
using wDNS.Processing;
using wDNS.Common.Extensions;

namespace wDNS.Listening;

public class Listener : IListener, IDisposable
{
    private readonly ILogger<Listener> _logger;
    private readonly IOptions<Configuration.Listening> _config;
    private readonly IOptions<Configuration.SuppressWarnings> _suppressed;

    private readonly IProcessor _processor;

    private readonly UdpClient _udp;
    private bool disposedValue;

    public delegate void ReceivedDelegate(object sender, byte[] buffer);
    public event ReceivedDelegate? Received;

    public Listener(ILogger<Listener> logger,
        IOptions<Configuration.Listening> config, IOptions<Configuration.SuppressWarnings> suppressed,
        IProcessor processor)
    {
        _logger = logger;
        _config = config;
        _suppressed = suppressed;

        _processor = processor;

        _udp = new UdpClient(_config.Value.Port);

        if (_config.Value.PrintBytesOnReceive)
        {
            Received += Listener_ReceivedLogEnabled;
        }
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

            Received?.Invoke(this, received);

            _logger.LogDebug("Processing DNS request from {Remote}", local);

            var receivedResult = new UdpReceiveResult(received, local);

#if SINGLETHREAD
            _processor.ProcessAsync(_udp, receivedResult, stoppingToken).GetAwaiter().GetResult();
#else
            Task.Run(async () => await _processor.ProcessAsync(_udp, receivedResult, stoppingToken), stoppingToken);
#endif
        }
    }

    private void Listener_ReceivedLogEnabled(object sender, byte[] buffer)
    {
        _logger.LogDebug("Received request with bytes\n{Buffer}", buffer.ToX2String());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _udp.Dispose();
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
