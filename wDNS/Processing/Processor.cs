using Microsoft.Extensions.Options;
using System.Net.Sockets;
using wDNS.Common;
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
    private readonly IKnowledgeOrchestrator _knowledge;
    private readonly IAnswerCache _cache;

    public event Request.OnReadDelegate QueryRead;

    public Processor(ILogger<Processor> logger, IOptions<Configuration.Processing> config, 
        IForwarder forwarder, IKnowledgeOrchestrator knowledge, IAnswerCache cache)
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

    public async Task ProcessAsync(UdpClient recipient, UdpReceiveResult udpResult, CancellationToken stoppingToken)
    {
        var query = BufferHelpers.ReadBuffer<Request>(udpResult.Buffer);

        _logger.LogDebug("Read query {{{Query}}}", query);
        QueryRead?.Invoke(this, udpResult.Buffer, query);

        var compiled = new List<Answer>(2);

        Response response;

        var questionResult = new QuestionResult();
        
        for (int i = 0; i < query.questions.Count; i++)
        {
            var question = query.questions[i];

            if (_knowledge.TryAnswer(question, questionResult))
            {
                _logger.LogDebug("Found knowledge for question {{{Question}}}: {{{Answer}}}", question, string.Join<Answer>(',', questionResult.Answers));
                compiled.AddRange(questionResult.Answers);
            }
            else
            {
                _logger.LogDebug("No knowledge found for question {{{Question}}}", question);
                response = await _forwarder.ForwardAsync(query, stoppingToken);

                _logger.LogDebug("Caching answers {{{Answers}}} for question {{{Question}}}", 
                    StringHelpers.Concatenate(response.answers), question);

                _cache.Add(question, response.answers);
                compiled.AddRange(response.answers);
            }
        }

        response = new Response(query, compiled, [], []);

        response.query.message.flags |= questionResult.Flags;

        response.query.message.Response = true;
        response.query.message.RecursionSupported = true;

        response.query.message.answerCount = (ushort)compiled.Count;

        _logger.LogDebug("Replying to request #{Identification} with response {Response}", response.query.message.identification, response);

        var buffer = BufferHelpers.WriteBuffer(response);

        try
        {
            await recipient.SendAsync(buffer, udpResult.RemoteEndPoint, stoppingToken);
        }
        catch (Exception ex)
        {
            var message = string.Format("Error while replying to query #{0}. Buffer: {1}", query.message.identification, buffer.Tox2String());
            throw new Exception(message, ex);
        }
    }

    private void Processor_QueryReadLogEnabled(object sender, byte[] buffer, Request query)
    {
        _logger.LogDebug("Read query #{Identification}'s buffer:\n{Buffer}", query.message.identification, buffer.Tox2String());
    }
}
