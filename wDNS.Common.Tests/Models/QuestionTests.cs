using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class QuestionTests
{
    [TestMethod]
    public void Write_ShouldReturn_NonEmptyBuffer()
    {
        var label = LabelHelpers.Create(Constants.Words);
        var ctx = BufferContextHelpers.Create();
        var emptyCtx = BufferContextHelpers.Create();

        var question = new Question()
        {
            name = label,
            @class = RecordClasses.IN,
            types = RecordTypes.A
        };

        question.Write(ctx);

        CollectionAssert.AreNotEquivalent(emptyCtx.buffer, ctx.buffer);
    }

    [TestMethod]
    public void WriteRead_NoLabel_ShouldReturn_SameProperties()
    {
        var ctx = BufferContextHelpers.Create();

        var write = new Question
        {
            name = new Label() { segments = [] },
            @class = RecordClasses.ANY,
            types = RecordTypes.AVC
        };
        write.Write(ctx);
        ctx.ResetPointer();

        var read = new Question();
        read.Read(ctx);

        Assert.AreEqual(write, read);
    }

    [TestMethod]
    public void WriteRead_ShouldReturn_SameQuestion()
    {
        var label = LabelHelpers.CreateFromConstants();
        var ctx = BufferContextHelpers.Create();

        var write = new Question()
        {
            name = label,
            @class = RecordClasses.ANY,
            types = RecordTypes.AVC
        };
        write.Write(ctx);
        ctx.ResetPointer();

        var read = new Question();
        read.Read(ctx);

        Assert.AreEqual(write, read);
    }
}
