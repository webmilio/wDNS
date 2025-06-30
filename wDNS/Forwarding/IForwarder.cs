using System.Threading.Channels;
using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Forwarding;

public interface IForwarder
{
    Task ForwardAsync(Request request, BufferContext result);
}
