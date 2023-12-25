using wDNS.Common.Models;

namespace wDNS.Common;

public interface IQuestionable
{
    /// <summary></summary>
    /// <param name="question"></param>
    /// <param name="result"></param>
    /// <returns><c>true</c> if the answer should be considered final; otherwise <c>false</c>.</returns>
    public bool TryAnswer(Question question, QuestionResult result);
}

public class QuestionResult
{
    public MessageFlags Flags { get; set; }
    public List<Answer> Answers { get; } = [];
}
