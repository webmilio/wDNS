namespace wDNS.Common.Attributes;

public class OrderAttribute : Attribute
{
    public OrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}
