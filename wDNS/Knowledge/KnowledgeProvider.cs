using wDNS.Common;
using wDNS.Common.Models;
using wDNS.Knowledge.Caching;

namespace wDNS.Knowledge;

public class KnowledgeProvider : IKnowledgeProvider
{
    private readonly List<IQuestionable> _sources = [];

    public KnowledgeProvider(IAnswerCache cache)
    {
        _sources.Add(cache);
    }

    /// <summary>Adds a knowledge source at the front of the pile.</summary>
    /// <param name="source"></param>
    public void Add(IQuestionable source)
    {
        _sources.Insert(0, source);
    }

    public bool PopulateAnswers(Question question, QuestionResult result)
    {
        bool found = false;

        for (int i = 0; i < _sources.Count; i++)
        {
            found |= _sources[i].PopulateAnswers(question, result);
        }

        return found;
    }
}
