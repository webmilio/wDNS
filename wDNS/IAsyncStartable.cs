namespace wDNS;

public interface IAsyncStartable
{
    Task StartAsync(CancellationToken cancellation);
}
