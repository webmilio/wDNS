using wDNS.Common.Models;

namespace wDNS.Knowledge;

public interface IEntryRegister
{
    void TryAddEntry(Question question, Response response);

    bool TryGetEntry(Question question, out Response response);

    bool ReadOnly { get; }
}
