using System.Net.Sockets;
using wDNS.Forwarding;
using wDNS.Models;

namespace wDNS.Processing;

public class RequestProcessor : IRequestProcessor
{
    private readonly IForwarder _forwarder;

    public RequestProcessor(IForwarder forwarder)
    {
        _forwarder = forwarder;
    }

    public async Task ProcessAsync(UdpClient recipient, UdpReceiveResult result, CancellationToken stoppingToken)
    {
        var ptr = 0;
        var query = Query.Read(result.Buffer, ref ptr);

        var response = await _forwarder.ForwardAsync(query, stoppingToken);

        ptr = 0;
        var buffer = new byte[Constants.UdpPacketMaxLength];
        response.Write(buffer, ref ptr);

        await recipient.SendAsync(buffer, result.RemoteEndPoint, stoppingToken);        
    }
}
