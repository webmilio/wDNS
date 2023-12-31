﻿using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Tests.Common;

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
        "example.com", RecordTypes.A, RecordClasses.IN
    })]
    public void Read(byte[] buffer,
        int id, int flags, int qdCount, int anCount, int nsCount, int arCount,
        string qName, RecordTypes qType, RecordClasses qClass)
    {
        int ptr = 0;
        var query = Request.Read(buffer, ref ptr);

        var message = query.message;

        Assert.AreEqual(id, message.identification);
        Assert.AreEqual((ushort)flags, (ushort)message.flags);
        Assert.AreEqual((ushort)flags, (ushort)message.flags);
        Assert.AreEqual((ushort)qdCount, message.questionCount);
        Assert.AreEqual((ushort)anCount, message.answerCount);
        Assert.AreEqual((ushort)nsCount, message.authorityCount);
        Assert.AreEqual((ushort)arCount, message.additionalCount);

        var questions = query.questions;

        Assert.AreEqual(questions.Count, message.questionCount);

        var question = questions[0];

        Assert.AreEqual(qName, question.name.Name);
        Assert.AreEqual(qType, question.type);
        Assert.AreEqual(qClass, question.@class);
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
        "example.com", RecordTypes.A, RecordClasses.IN
    })]
    public void Write(byte[] buffer,
        int id, int flags, int qdCount, int anCount, int nsCount, int arCount,
        string qName, RecordTypes qType, RecordClasses qClass)
    {
        int ptr = 0;
        var query = Request.Read(buffer, ref ptr);

        ptr = 0;
        query.Write(buffer, ref ptr);

        Read(buffer, id, flags, qdCount, anCount, nsCount, arCount, qName, qType, qClass);
    }
}
