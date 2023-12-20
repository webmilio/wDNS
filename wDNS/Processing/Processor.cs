using System.Net.Sockets;
using System.Text;
using wDNS.Caching;
using wDNS.Common;
using wDNS.Forwarding;

namespace wDNS.Processing;

public class Processor : IProcessor
{
    private readonly ILogger<Processor> _logger;
    private readonly IForwarder _forwarder;
    private readonly IAnswerCache _cachedAnswers;

    public Processor(ILogger<Processor> logger,
        IForwarder forwarder, IAnswerCache cachedAnswers)
    {
        _logger = logger;
        _forwarder = forwarder;
        _cachedAnswers = cachedAnswers;
    }

    public async Task ProcessAsync(UdpClient recipient, UdpReceiveResult result, CancellationToken stoppingToken)
    {
        var ptr = 0;

        var buffer = result.Buffer;
        var query = Query.Read(buffer, ref ptr);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            var sb = new StringBuilder(buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                sb.AppendFormat("{0} ", buffer[i].ToString("X2"));
            }

            _logger.LogTrace("Request {{{Questions}}} bytes: {Bytes}", string.Join<Question>(',', query.Questions), sb);
        }

        var compiled = new List<Answer>(2);
        Response response;

        for (int i = 0; i < query.Questions.Count; i++)
        {
            var question = query.Questions[i];

            if (_cachedAnswers.TryGet(question, out var cached))
            {
                _logger.LogDebug("Found cached answers for question {{{Question}}}: {{{Answer}}}", question, string.Join<Answer>(',', cached));
                compiled.AddRange(cached);
            }
            else
            {
                _logger.LogDebug("No answer found for question {{{Question}}}.", question);
                response = await _forwarder.ForwardAsync(query, stoppingToken);

                _logger.LogDebug("Caching answers {{{Answers}}} for question {{{Question}}}.", string.Join(',', response.Answers), question);
                _cachedAnswers.Add(question, response.Answers);

                compiled.AddRange(response.Answers);
            }
        }
 
        response = new Response()
        {
            Message = query.Message,
            Questions = query.Questions,
            Answers = compiled,
            Authorities = [],
            Additional = []
        };

        response.Message.AnswerCount = (ushort)compiled.Count;

        _logger.LogDebug("Replying to request ID {Identification} with response {Response}", response.Message.Identification, response);

        ptr = 0;
        buffer = new byte[Constants.UdpPacketMaxLength];
        response.Write(buffer, ref ptr);

        Array.Resize(ref buffer, ptr);

        await recipient.SendAsync(buffer, result.RemoteEndPoint, stoppingToken);        
    }
}
