using wDNS.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class QueryTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[]
        {
            // DNS Message
            148, 99,    // Transaction ID (37987)
            1, 0,       // Flags (b0000_0001_0000_0000)
            0, 1,       // Question Count (1)
            0, 0,       // Answer Count (0)
            0, 0,       // Authority Count (0)
            0, 1,       // Additional Count (1)

            // Question
            7,          // QName Start
                        // Label length: 7
            101,        // e
            120,        // x
            97,         // a
            109,        // m
            112,        // p
            108,        // l
            101,        // e
            3,          // Label length: 3
            99,         // c
            111,        // o
            109,        // m
            0,          // QName Termination
            0, 1,       // QType: A
            0, 1        // QClass: IN
        },
        37987, 0x0100, 0x0001, 0x0000, 0x0000, 0x0001,
        "example.com", QTypes.A, QClasses.IN
    })]
    public void Read(byte[] buffer,
        int id, int flags, int qdCount, int anCount, int nsCount, int arCount,
        string qName, QTypes qType, QClasses qClass)
    {
        int ptr = 0;
        var query = Query.Read(buffer, ref ptr);

        var message = query.Message;

        Assert.AreEqual(id, message.Identification);
        Assert.AreEqual((ushort)flags, (ushort)message.Flags);
        Assert.AreEqual((ushort)flags, (ushort)message.Flags);
        Assert.AreEqual((ushort)qdCount, message.QuestionCount);
        Assert.AreEqual((ushort)anCount, message.AnswerCount);
        Assert.AreEqual((ushort)nsCount, message.AuthorityCount);
        Assert.AreEqual((ushort)arCount, message.AdditionalCount);

        var questions = query.Questions;

        Assert.AreEqual(questions.Length, message.QuestionCount);

        var question = questions[0];

        Assert.AreEqual(qName, question.QName);
        Assert.AreEqual(qType, question.QType);
        Assert.AreEqual(qClass, question.QClass);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[]
        {
            // DNS Message
            148, 99,    // Transaction ID (37987)
            1, 0,       // Flags (b0000_0001_0000_0000)
            0, 1,       // Question Count (1)
            0, 0,       // Answer Count (0)
            0, 0,       // Authority Count (0)
            0, 1,       // Additional Count (1)

            // Question
            7,          // QName Start
                        // Label length: 7
            101,        // e
            120,        // x
            97,         // a
            109,        // m
            112,        // p
            108,        // l
            101,        // e
            3,          // Label length: 3
            99,         // c
            111,        // o
            109,        // m
            0,          // QName Termination
            0, 1,       // QType: A
            0, 1        // QClass: IN
        },
        37987, 0x0100, 0x0001, 0x0000, 0x0000, 0x0001,
        "example.com", QTypes.A, QClasses.IN
    })]
    public void Write(byte[] buffer,
        int id, int flags, int qdCount, int anCount, int nsCount, int arCount,
        string qName, QTypes qType, QClasses qClass)
    {
        int ptr = 0;
        var query = Query.Read(buffer, ref ptr);

        ptr = 0;
        query.Write(buffer, ref ptr);

        Read(buffer, id, flags, qdCount, anCount, nsCount, arCount, qName, qType, qClass);
    }
}
