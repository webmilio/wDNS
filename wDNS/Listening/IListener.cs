namespace wDNS.Listening;

public interface IListener
{
    public void Listen(CancellationToken stoppingToken);
}
