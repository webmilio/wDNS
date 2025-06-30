using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using wDNS.Common.Models;
using wDNS.Forwarding;

namespace wDNS.Knowledge;

public class ForwardResponseRegister : IEntryRegister
{
    private readonly IMemoryCache _cache;

    public ForwardResponseRegister(IMemoryCache cache, IForwarder forwarder)
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public void TryAddEntry(Question question, Response response)
    {
        _cache.Set($"{GetPrefix()}_{question.name}", response, TimeSpan.FromSeconds(3600));
    }

    public bool TryGetEntry(Question question, out Response response)
    {
        return _cache.TryGetValue($"{GetPrefix()}_{question.name}", out response);
    }

    public bool ReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetPrefix() => nameof(ForwardResponseRegister);
}
