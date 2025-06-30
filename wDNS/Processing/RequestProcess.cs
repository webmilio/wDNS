using Microsoft.Extensions.Options;
using System.Net.Sockets;
using wDNS.Common;
using wDNS.Common.Models;
using wDNS.Conversion.Readable;
using wDNS.Extensions;
using wDNS.Forwarding;
using wDNS.Knowledge;

namespace wDNS.Processing;

public class RequestProcess : IRequestProcessor
{
    private readonly IOptions<Configuration.Processing> _processingOpt;
    private readonly ILogger _logger;
    private readonly IReadableStringConverter<Request> _readable;
    private readonly IEnumerable<IEntryRegister> _registers;
    private readonly IForwarder _forwarder;

    public RequestProcess(IOptions<Configuration.Processing> processOpt, ILogger<RequestProcess> logger, IReadableStringConverter<Request> readable,
        IEnumerable<IEntryRegister> registers, IForwarder forwarder)
    {
        _processingOpt = processOpt;
        _logger = logger;
        _readable = readable;
        _registers = registers;
        _forwarder = forwarder;
    }

    public Task<BufferContext> ProcessAsync(BufferContext context)
    {
        var request = context.Read<Request>();
        return ProcessAsync(request);
    }

    public async Task<BufferContext> ProcessAsync(Request request)
    {
        var requestStr = string.Empty;

        if (_logger.IsEnabled(LogLevel.Debug) && _processingOpt.Value.PrintRequestOnProcess)
        {
            requestStr = _readable.GetReadableString(request);
        }
        _logger.LogDebug("Processing request id {Id}: {Request}", request.message.id, requestStr);

        var response = BufferContext.CreateUdpContext();
        await _forwarder.ForwardAsync(request, response);

        return response;

        foreach (var register in _registers)
        {
            
        }
    }
}
