using System.Net.Sockets;
using wDNS.Models;

namespace wDNS.Processing;

public interface IRequestProcessor
{
    public Task ProcessAsync(UdpClient recipient, UdpReceiveResult result, CancellationToken stoppingToken);
}
