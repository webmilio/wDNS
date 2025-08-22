namespace wDNS.Common.Tests.Helpers;

public class BufferContextHelpers
{
    public static BufferContext Create() => BufferContext.CreateUdpContext();

    public static BufferContext CreateWithLabel(params string[] words)
    {
        var context = Create();
        var label = LabelHelpers.Create(words);

        label.Write(context);
        context.ResetPointer();
        
        return context;
    }
}
