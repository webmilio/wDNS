using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Knowledge.Caching;

public interface IAnswerCache : IQuestionable
{
    public void Add(Question question, IList<Answer> answers);
}
