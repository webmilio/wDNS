
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using wDNS.Common;
using wDNS.Extensions;
using wDNS.Processing;

namespace wDNS.Listening;

public class UdpListener : IUdpListener
{
    private readonly ILogger<UdpListener> _logger;
    private readonly IOptions<Configuration.Listening> _listOpt;
    private readonly IOptions<Configuration.SuppressWarnings> _suppressOpt;
    private readonly IRequestProcessor _processor;

    public UdpListener(
        ILogger<UdpListener> logger,
        IOptions<Configuration.Listening> listOpt,
        IOptions<Configuration.SuppressWarnings> suppressOpt,
        IRequestProcessor processor)
    {
        _logger = logger;
        _listOpt = listOpt;
        _suppressOpt = suppressOpt;

        _processor = processor;
    }

    public async Task StartAsync(CancellationToken cancellation)
    {
        _logger.MethodCall();

        var listOpt = _listOpt.Value;
        var endpoint = new IPEndPoint(listOpt._IPAddress, listOpt.Port);

        using var udp = new UdpClient(endpoint);

        _logger.InstancedWithProperties<UdpClient>([(nameof(UdpClient.Client.ReceiveBufferSize), udp.Client.ReceiveBufferSize)]);
        _logger.LogInformation("{ListenerType} listening on port {Endpoint}", udp.GetType().FullName, endpoint);

        while (!cancellation.IsCancellationRequested)
        {
            UdpReceiveResult received;

            try
            {
                received = await udp.ReceiveAsync(cancellation);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionReset)
            {
                if (!_suppressOpt.Value.UDPConnectionReset)
                {
                    _logger.LogError("Error while receiving: the remote connection was closed.");
                }
                return;
            }

            await ProcessReceived(udp, received);

            //await Task.Factory.StartNew(async () => await ProcessReceived(udp, received), cancellation, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }
    }

    private async Task ProcessReceived(UdpClient client, UdpReceiveResult received)
    {
        if (_listOpt.Value.PrintBytesOnReceive)
        {
            _logger.LogTrace($"Received {{Length}} bytes request from {{{nameof(UdpReceiveResult.RemoteEndPoint)}}}: {{Bytes}}", received.Buffer.Length, received.RemoteEndPoint, received.Buffer.ToX2String());
        }

        var context = new BufferContext(received.Buffer);
        var response = await _processor.ProcessAsync(context);

        await client.SendAsync(response.buffer, received.RemoteEndPoint);
    }
}
