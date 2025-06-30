using wDNS.Common.Models;

namespace wDNS.Common.Tests.Helpers;

public static class LabelHelpers
{
    public static Label Create(params string[] words)
    {
        var label = new Label
        {
            segments = [.. words.Select(x => new LabelSegment(x))]
        };

        return label;
    }

    public static Label CreateFromConstants() => Create(Constants.Words);
}
