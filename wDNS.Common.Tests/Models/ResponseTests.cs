using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class ResponseTests
{
    private static readonly byte[] ValidGoogleResponse_ExampleCom = [148, 99, 129, 128, 0, 1, 0, 6, 0, 0, 0, 0, 7, 101, 120, 97, 109, 112, 108, 101, 3, 99, 111, 109, 0, 0, 1, 0, 1, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 23, 215, 0, 136, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 23, 215, 0, 138, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 23, 192, 228, 80, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 23, 192, 228, 84, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 96, 7, 128, 198, 192, 12, 0, 1, 0, 1, 0, 0, 0, 57, 0, 4, 96, 7, 128, 175];

    [TestMethod]
    public void DeserializedValidGoogleResponse_ReturnsValid()
    {
        var ctx = new BufferContext(ValidGoogleResponse_ExampleCom);
        var response = ctx.Read<Response>();

        ;
    }

    [TestMethod]
    public void DeserializedValidGoogleResponse_ReturnsValidMessage()
    {
        var ctx = new BufferContext(ValidGoogleResponse_ExampleCom);
        var message = ctx.Read<Message>();

        Assert.AreNotEqual(0, message.id);
        Assert.AreNotEqual(0, (ushort) message.flags);
        Assert.AreEqual(1, message.questionCount);
        Assert.AreNotEqual(0, message.answerCount);
    }

    [TestMethod]
    public void DeserializedValidGoogleResponse_ReturnsValidQuestion()
    {
        var ctx = new BufferContext(ValidGoogleResponse_ExampleCom);

        _ = ctx.Read<Message>();
        var question = ctx.Read<Question>();

        Assert.AreEqual(2, question.name.segments.Length);
        Assert.AreEqual(RecordTypes.A, question.types);
        Assert.AreEqual(RecordClasses.IN, question.@class);
    }

    [TestMethod]
    public void DeserializedValidGoogleResponse_ReturnsValidAnswer()
    {
        var ctx = new BufferContext(ValidGoogleResponse_ExampleCom);

        var message = ctx.Read<Message>();
        _ = ctx.Read<Question>();
        var answers = new Answer[message.answerCount];

        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].Read(ctx);
        }

        CollectionAssert.DoesNotContain(answers, default);
    }
}
