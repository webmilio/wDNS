using System.Reflection;
using wDNS.Common;
using wDNS.Common.Attributes;

namespace wDNS.Knowledge;

public class KnowledgeOrchestrator(ILogger<KnowledgeOrchestrator> logger, IKnowledgeOrchestrator knowledge, IServiceProvider services)
{
    internal Task Initialize()
    {
        logger.LogInformation("Initializing knowledge root");
        IEnumerable<IQuestionable> questionables = services.GetServices<IQuestionable>();

        questionables = questionables.OrderBy(Order);

        foreach (var questionable in questionables)
        {
            logger.LogDebug("Registering {{{Class}}} as a knowledge source", questionable);
            knowledge.Add(questionable);
        }

        return knowledge.Initialize();
    }

    private int Order(IQuestionable questionable)
    {
        var order = questionable.GetType().GetCustomAttribute<OrderAttribute>();
        return order?.Order ?? int.MaxValue / 2;
    }
}
