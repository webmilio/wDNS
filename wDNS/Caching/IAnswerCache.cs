using wDNS.Common;

namespace wDNS.Caching;

public interface IAnswerCache
{
    public bool TryGet(Question question, out Answer[] answers);

    public void Add(Question question, IList<Answer> answers);
}
