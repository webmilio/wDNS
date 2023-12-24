using Microsoft.Extensions.Options;
using System.Net.Sockets;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;
using wDNS.Common.Models;
using wDNS.Forwarding;
using wDNS.Knowledge;
using wDNS.Knowledge.Caching;

namespace wDNS.Processing;

public class Processor : IProcessor
{
    private readonly ILogger<Processor> _logger;
    private readonly IOptions<Configuration.Processing> _config;

    private readonly IForwarder _forwarder;
    private readonly IKnowledgeProvider _knowledge;
    private readonly IAnswerCache _cache;

    public event Query.OnReadDelegate QueryRead;

    public Processor(ILogger<Processor> logger, IOptions<Configuration.Processing> config, 
        IForwarder forwarder, IKnowledgeProvider knowledge, IAnswerCache cache)
    {
        _logger = logger;
        _config = config;

        _forwarder = forwarder;
        _knowledge = knowledge;
        _cache = cache;

        if (_config.Value.PrintBytesOnReceive)
        {
            QueryRead += Processor_QueryReadLogEnabled;
        }
    }

    public async Task ProcessAsync(UdpClient recipient, UdpReceiveResult result, CancellationToken stoppingToken)
    {
        var ptr = 0;

        var buffer = result.Buffer;
        var query = Query.Read(buffer, ref ptr);

        _logger.LogDebug("Read query {{{Query}}}", query);
        QueryRead?.Invoke(this, buffer, ptr, query);

        var compiled = new List<Answer>(2);
        Response response;

        for (int i = 0; i < query.Questions.Count; i++)
        {
            var question = query.Questions[i];

            if (_knowledge.TryAnswer(question, out var knowledge))
            {
                _logger.LogDebug("Found knowledge for question {{{Question}}}: {{{Answer}}}", question, string.Join<Answer>(',', knowledge));
                compiled.AddRange(knowledge);
            }
            else
            {
                _logger.LogDebug("No knowledge found for question {{{Question}}}", question);
                response = await _forwarder.ForwardAsync(query, stoppingToken);

                _logger.LogInformation("Caching answers for question {{{Question}}}", question);
                _logger.LogDebug("Caching answers {{{Answers}}} for question {{{Question}}}", 
                    StringHelpers.Concatenate(response.Answers), question);

                _cache.Add(question, response.Answers);
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

        response.Message.Flags |= Common.MessageFlags.Query_Response;
        response.Message.AnswerCount = (ushort)compiled.Count;

        _logger.LogDebug("Replying to request #{Identification} with response {Response}", response.Message.Identification, response);

        buffer = BufferHelpers.WriteBuffer(response);

        try
        {
            await recipient.SendAsync(buffer, result.RemoteEndPoint, stoppingToken);
        }
        catch (Exception ex)
        {
            var message = string.Format("Error while replying to query #{0}. Buffer:\n{1}", query.Message.Identification, buffer);
            throw new Exception(message, ex);
        }
    }

    private void Processor_QueryReadLogEnabled(object sender, byte[] buffer, int length, Query query)
    {
        _logger.LogDebug("Read query #{Identification}'s buffer:\n{Buffer}",
            query.Message.Identification, buffer.ToX2String(0, length));
    }
}
