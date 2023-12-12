namespace wDNS.Listening;

public interface IRequestListener
{
    public void Listen(CancellationToken stoppingToken);
}
