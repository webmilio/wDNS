using wDNS.Common.Models;

namespace wDNS.Common;

public interface IQuestionable
{
    public bool PopulateAnswers(Question question, QuestionResult result);
}

public class QuestionResult
{
    public MessageFlags Flags { get; set; }
    public List<Answer> Answers { get; } = [];
}
