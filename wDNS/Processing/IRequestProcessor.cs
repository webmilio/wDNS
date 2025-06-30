using wDNS.Common;

namespace wDNS.Processing;

public interface IRequestProcessor
{
    Task<BufferContext> ProcessAsync(BufferContext context);
}
