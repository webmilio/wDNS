using wDNS.Common;

namespace wDNS.Knowledge;

public interface IKnowledgeStore : IQuestionable
{
    public Task Initialize();

    public void Add(IQuestionable source);
}
