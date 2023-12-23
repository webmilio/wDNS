﻿using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;
using wDNS.Caching;
using wDNS.Common;
using wDNS.Common.Extensions;
using wDNS.Common.Models;
using wDNS.Forwarding;

namespace wDNS.Processing;

public class Processor : IProcessor
{
    private readonly ILogger<Processor> _logger;
    private readonly IForwarder _forwarder;
    private readonly IAnswerCache _cachedAnswers;
    private readonly IOptions<Configuration.Processing> _config;
    private bool disposedValue;

    public event Query.OnReadDelegate QueryRead;

    public Processor(ILogger<Processor> logger, IOptions<Configuration.Processing> config, 
        IForwarder forwarder, IAnswerCache cachedAnswers)
    {
        _logger = logger;
        _config = config;

        _forwarder = forwarder;
        _cachedAnswers = cachedAnswers;

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

            if (_cachedAnswers.TryGet(question, out var cached))
            {
                _logger.LogDebug("Found cached answers for question {{{Question}}}: {{{Answer}}}", question, string.Join<Answer>(',', cached));
                compiled.AddRange(cached);
            }
            else
            {
                _logger.LogDebug("No answer found for question {{{Question}}}", question);
                response = await _forwarder.ForwardAsync(query, stoppingToken);

                _logger.LogDebug("Caching answers {{{Answers}}} for question {{{Question}}}", 
                    Helpers.Concatenate(response.Answers), question);
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

        _logger.LogDebug("Replying to request #{dentification} with response {Response}", response.Message.Identification, response);

        buffer = Helpers.WriteBuffer(response);
        await recipient.SendAsync(buffer, result.RemoteEndPoint, stoppingToken);        
    }

    private void Processor_QueryReadLogEnabled(object sender, byte[] buffer, int length, Query query)
    {
        _logger.LogDebug("Read query #{Identification}'s buffer:\n{Buffer}",
            query.Message.Identification, buffer.ToX2String(0, length));
    }
}
