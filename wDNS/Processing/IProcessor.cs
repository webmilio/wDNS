using System.Net.Sockets;

namespace wDNS.Processing;

public interface IProcessor
{
    public Task ProcessAsync(UdpClient recipient, UdpReceiveResult result, CancellationToken stoppingToken);
}
