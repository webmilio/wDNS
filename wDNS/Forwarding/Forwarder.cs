using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Forwarding;

public class Forwarder : IForwarder, IDisposable
{
    private readonly ILogger<Forwarder> _logger;
    private readonly IOptions<Configuration.Forwarding> _config;

    private readonly UdpClient _udp;
    private readonly IPEndPoint[] _remotes;

    private bool disposedValue;

    public delegate void ReceivedDelegate(object sender, Query src, byte[] buffer);
    public event ReceivedDelegate Received;

    public event Query.Delegate? Forwarding;
    public event Response.FromQuestionDelegate? Read;

    public Forwarder(ILogger<Forwarder> logger, IOptions<Configuration.Forwarding> config)
    {
        _logger = logger;
        _config = config;

        _udp = new UdpClient(_config.Value.Port);
        _remotes = _config.Value.GetRemotes();

        if (_config.Value.PrintResponseBytesOnReceive)
        {
            Received += Forwarder_ReceivedLogEnabled;
        }
    }

    public async Task<Response> ForwardAsync(Query query, CancellationToken stoppingToken)
    {
        const string DisabledInConfig = "Disabled in config"; // Disabled in configuration file.

        if (_config.Value.Timeout > 0)
        {
            var cancelSource = new CancellationTokenSource();
            stoppingToken.Register(cancelSource.Cancel);

            cancelSource.CancelAfter(_config.Value.Timeout);
            stoppingToken = cancelSource.Token;
        }

        var buffer = BufferHelpers.WriteBuffer(query);

        var remote = _remotes[0]; // TODO Change this to use multiple servers.
        _logger.LogDebug("Forwarding request #{Identification} to {Remote}", query.Message.Identification, remote);

        Forwarding?.Invoke(this, query);

        try
        {
            await _udp.SendAsync(buffer, remote, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout encountered while forwarding request #{Identification} to {Remote}", 
                query.Message.Identification, remote);
            throw;
        }

        UdpReceiveResult received;
        try
        {
            received = await _udp.ReceiveAsync(stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout encountered while received forwarded request #{Identification} to {Remote}",
                query.Message.Identification, remote);
            throw;
        }

        Received?.Invoke(this, query, received.Buffer);

        int ptr = 0;
        Response response;

        try
        {
            response = Response.Read(received.Buffer, ref ptr);
        }
        catch (Exception ex)
        {
            var conf = _config.Value;
            var sQuery = DisabledInConfig;
            
            if (conf.PrintQueryBytesOnReceiveError)
            {
                var mBuffer = BufferHelpers.WriteBuffer(query);
                sQuery = mBuffer.ToX2String();
            }

            _logger.LogError(ex, "Error while reading forwarded query #{Identification} response." +
                "\n\tQuery Buffer: {QueryBuffer}\n\tResponse Buffer: {Response}",
                query.Message.Identification, sQuery, 
                conf.PrintResponseBytesOnReceiveError ? received.Buffer.ToX2String() : DisabledInConfig);
            throw;
        }

        Read?.Invoke(this, query, received.Buffer, response);

        _logger.LogDebug("Received forwarded request {{{Questions}}} response from {Remote}: {Response}",
            string.Join(',', query.Questions), remote, response);

        return response;
    }

    private void Forwarder_ReceivedLogEnabled(object sender, Query src, byte[] buffer)
    {
        _logger.LogDebug("Received forwarded query #{Identification} response buffer:\n{Buffer}\nLength of {Length}",
            src.Message.Identification, buffer.ToX2String(), buffer.Length);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _udp.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
