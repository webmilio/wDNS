using wDNS.Common;

namespace wDNS.Knowledge;

public interface IKnowledgeProvider : IQuestionable
{
    public void Add(IQuestionable source);
}
