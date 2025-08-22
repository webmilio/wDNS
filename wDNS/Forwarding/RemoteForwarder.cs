using Microsoft.Extensions.Options;
using System.Net.Sockets;
using wDNS.Common;
using wDNS.Common.Models;
using wDNS.Conversion.Readable;
using wDNS.Extensions;

namespace wDNS.Forwarding;

public class RemoteForwarder : IForwarder
{
    private readonly IOptions<Configuration.Forwarding> _forwardOpt;
    private readonly ILogger _logger;

    private readonly IReadableStringConverter<Request> _requestConverter;
    private readonly IReadableStringConverter<Response> _responseConverter;

    private readonly Random _rng = new();

    public RemoteForwarder(IOptions<Configuration.Forwarding> forwardOpt, ILogger<RemoteForwarder> logger, IReadableStringConverter<Request> requestConverter, IReadableStringConverter<Response> responseConverter)
    {
        _forwardOpt = forwardOpt;
        _logger = logger;

        _requestConverter = requestConverter;
        _responseConverter = responseConverter;
    }

    public async Task ForwardAsync(Request request, BufferContext result)
    {
        var remotes = _forwardOpt.Value.GetRemotes();

        foreach (var remote in remotes)
        {
            var remoteRequest = BufferContext.CreateUdpContext();
            remoteRequest.Write(request);

            var ports = _rng.Next(_forwardOpt.Value.Ports[0], _forwardOpt.Value.Ports[1] + 1);
            using var client = new UdpClient(ports, remote.AddressFamily);

            byte[] forwardResponse;

            try
            {
                var questionStr = string.Empty;
                if (_forwardOpt.Value.PrintRequestBytesOnSending)
                {
                    questionStr = request.ToBuffer().ToX2String();
                }

                _logger.LogTrace("Forwarding question id {Id} to {Remote}: {Question}", request.message.id, remote, questionStr);
                await client.SendAsync(remoteRequest.buffer, remoteRequest.pointer, remotes[0]);

                _logger.LogTrace("Waiting for question id {Id}'s response from remote {Remote}", request.message.id, remote);
                
                var remoteResponse = await client.ReceiveAsync();
                forwardResponse = remoteResponse.Buffer;

                if (_forwardOpt.Value.PrintResponseBytesOnReceive)
                {
                    _logger.LogTrace("Received buffer for request id {Id} from remote {Remote}: {Buffer}", request.message.id, remote, forwardResponse.ToX2String());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while forwarding request to remote {Remote}", remote);
                return;
            }

            var responseStr = string.Empty;

            if (_forwardOpt.Value.PrintResponseOnReceive)
            {
                var responseContext = new BufferContext(forwardResponse);
                var response = responseContext.Read<Response>();

                responseStr = _responseConverter.GetReadableString(response);
            }

            _logger.LogTrace("Received response for request id {Id} from remote {Remote}: {Response}", request.message.id, remote, responseStr);

            result.WriteArray(forwardResponse);
        }
    }
}
