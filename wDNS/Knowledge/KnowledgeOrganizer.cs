using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Knowledge;

public class KnowledgeOrganizer : IKnowledgeOrchestrator
{
    protected readonly List<IQuestionable> sources = [];

    public virtual async Task Initialize()
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i] is IKnowledgeStore ks)
            {
                await ks.Initialize();
            }
        }
    }

    /// <summary>Adds a knowledge source at the front of the pile.</summary>
    /// <param name="source"></param>
    public void Add(IQuestionable source)
    {
        sources.Insert(0, source);
    }

    public bool TryAnswer(Question question, QuestionResult result)
    {
        bool found = false;

        for (int i = 0; i < sources.Count; i++)
        {
            found |= sources[i].TryAnswer(question, result);
        }

        return found;
    }
}
