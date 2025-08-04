using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class ResponseTests
{
    private static readonly byte[] ValidGoogleResponse_ExampleCom = [
        // Message
        148, 99, // Id = 37987
        129, 128, // Flags = 32896, 1000_0000_1000_0000 - Response, Supported, 
        0, 1, // Question Count = 1
        0, 6, // Answer Count = 6
        0, 0, // Authority Count
        0, 0, // Additional Count

        // Question - idx 12
        7, // Label length = 7
        101, // e
        120, // x
        97, // a
        109, // m
        112, // p
        108, // l
        101, // e
        3, // Label length = 3
        99, // c
        111, // o
        109, // m
        0, // Label terminator

        // idx 25
        0, 1, // Record Type = A
        0, 1, // Record Class = IN

        // Answer - idx 29
        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        23, 215, 0, 136, // RDATA = 23.215.0.136

        // idx 46
        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        23, 215, 0, 138, // RDATA = 23.215.0.138

        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        23, 192, 228, 80, // RDATA = 23.192.228.80

        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        23, 192, 228, 84, // RDATA = 23.192.228.80
        
        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        96, 7, 128, 198, // RDATA = 96.7.128.198

        192, 12, // Pointer = 12
        0, 1, // Record Type = A
        0, 1, // Record Class = IN
        0, 0, 0, 57, // Time to live = 57
        0, 4, // RDLENGTH = 4
        96, 7, 128, 175 // RDATA = 96.7.128.175
    ];

    [TestMethod]
    public void DeserializedValidGoogleResponse_ReturnsValid()
    {
        var ctx = new BufferContext(ValidGoogleResponse_ExampleCom);
        var response = ctx.Read<Response>();

        Assert.IsNotNull(response);
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
        Assert.AreEqual(12, ctx.pointer);
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
        Assert.AreEqual(29, ctx.pointer);
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
