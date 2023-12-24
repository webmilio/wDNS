using wDNS.Common.Models;

namespace wDNS.Common;

public interface IQuestionable
{
    public bool TryAnswer(Question question, out IList<Answer>? answers);
}
