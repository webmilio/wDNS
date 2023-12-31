﻿using wDNS.Common;
using wDNS.Common.Extensions;
using wDNS.Common.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class ResponseTests
{
    [DataTestMethod]
    [DataRow(new object[] { new byte[] { 0x8c, 0x56, 0x81, 0x80, 0x00, 0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x06, 0x61, 0x73, 0x73, 0x65, 0x74, 0x73, 0x03, 0x6d, 0x73, 0x6e, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x05, 0x00, 0x01, 0x00, 0x00, 0x04, 0x8a, 0x00, 0x1c, 0x06, 0x61, 0x73, 0x73, 0x65, 0x74, 0x73, 0x03, 0x6d, 0x73, 0x6e, 0x03, 0x63, 0x6f, 0x6d, 0x07, 0x65, 0x64, 0x67, 0x65, 0x6b, 0x65, 0x79, 0x03, 0x6e, 0x65, 0x74, 0x00, 0xc0, 0x2c, 0x00, 0x05, 0x00, 0x01, 0x00, 0x00, 0x03, 0x1e, 0x00, 0x16, 0x06, 0x65, 0x32, 0x38, 0x35, 0x37, 0x38, 0x01, 0x64, 0x0a, 0x61, 0x6b, 0x61, 0x6d, 0x61, 0x69, 0x65, 0x64, 0x67, 0x65, 0xc0, 0x43, 0xc0, 0x54, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x17, 0x21, 0xee, 0x2b, 0xc0, 0x54, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x17, 0x21, 0xee, 0x38, 0xc0, 0x54, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x17, 0x21, 0xee, 0x61 } }, DisplayName = "Infinite Loop")]
    public void NoThrowing(byte[] buffer)
    {
        int ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        Assert.IsTrue(true);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] {
            // DNS MESSAGE
            0x77, 0x22,
            0x81, 0x80,
            0x00, 0x01,
            0x00, 0x01,
            0x00, 0x00,
            0x00, 0x00, 
            
            // QUESTION
            // QName
/*b12*/     0x04,
            0x62, 0x69, 0x6E, 0x67,
            0x03,
            0x63, 0x6F, 0x6D, 
/*b21*/     0x00, // End
            
            
            0x00, 0x1C, // QType
            0x00, 0x01, // QClass

            // ANSWER
/*b26*/     0xC0, 0x0C, // QName Pointer 
            0x00, 0x1C, // QType
            0x00, 0x01, // QClass
            0x00, 0x00, 0x05, 0x8E, // TTL

/*b36*/     0x00, 0x10, // Data Length
/*b38*/     0x26, 0x20, 0x01, 0xEC, 0x0C, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 
            
/*b54*/     0xCC, 0x4F, 0xC5, 0xC8
        },
        "bing.com", RecordTypes.AAAA, RecordClasses.IN, 1422, 16,
        new byte[] { 0x26, 0x20, 0x01, 0xEC, 0x0C, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00 }
    })]
    public void Receive(byte[] buffer, string qName, RecordTypes qType, RecordClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        int ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        Equal(response.answers[0], qName, qType, qClass, ttl, rdLength, rData);
    }

    [DataTestMethod]
    [DataRow(new object[] { new byte[]
    {
        // Message
        0x00, 0x01,         // Identification: 1
        0x00, 0x00,         // Flags: 0
        0x00, 0x01,         // Question Count: 1
        0x00, 0x02,         // Answer Count: 2
        0x00, 0x00,         // Authority Count: 0
        0x00, 0x00,         // Additional Count: 0

        // Question
            // QName
        0x06,               // QName Length: 6
            // foobar
        0x66, 0x6F, 0x6F, 0x62, 0x61, 0x72,
        0x03,               // QName Length: 3
            // com
        0x63, 0x6F, 0x6D,
        0x00,               // Label End
        0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

        // Answers
        0xC0, 0x0C,         // QName Pointer: 12
        0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

            // TTL: 255
        0x00, 0x00, 0x00, 0xFF,
        
        0x00, 0x04,         // Data Length: 4
            // Data: 192.99.17.96
        0xC0, 0x63, 0x11, 0x60,

        // Answers
        0xC0, 0x0C,         // QName Pointer: 12
        0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

            // TTL: 255
        0x00, 0x00, 0x00, 0xFF,

        0x00, 0x04,         // Data Length: 4
            // Data: 192.99.17.3
        0xC0, 0x63, 0x11, 0x03
    } }, DisplayName = "Manual Buffer")]
    [DataRow(new object[] { new byte[] { 0x80, 0xcd, 0x81, 0x80, 0x00, 0x01, 0x00, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x14, 0x61, 0x75, 0x64, 0x69, 0x6f, 0x2d, 0x61, 0x6b, 0x2d, 0x73, 0x70, 0x6f, 0x74, 0x69, 0x66, 0x79, 0x2d, 0x63, 0x6f, 0x6d, 0x09, 0x61, 0x6b, 0x61, 0x6d, 0x61, 0x69, 0x7a, 0x65, 0x64, 0x03, 0x6e, 0x65, 0x74, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x05, 0x00, 0x01, 0x00, 0x00, 0x00, 0x38, 0x00, 0x13, 0x04, 0x61, 0x32, 0x39, 0x37, 0x04, 0x64, 0x73, 0x63, 0x63, 0x06, 0x61, 0x6b, 0x61, 0x6d, 0x61, 0x69, 0xc0, 0x2b, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0xa0, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x99, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x92, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x91, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0xa8, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x98, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0xa9, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x89, 0xc0, 0x40, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x04, 0x18, 0xc8, 0x00, 0x9b } })]
    public void SymmetricResponse(byte[] buffer)
    {
        int ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        ptr = 0;

        var resultBuffer = new byte[255];
        response.Write(resultBuffer, ref ptr);

        Array.Resize(ref resultBuffer, ptr);

        Assert.AreEqual(buffer.Length, resultBuffer.Length);
        CollectionAssert.AreEqual(buffer, resultBuffer);
    }

    [TestMethod]
    public void FullReceiveTest1()
    {
        var buffer = _spotifyDnsResponse;

        const int QuestionCount = 1;
        const string QName = "audio-ak-spotify-com.akamaized.net";

        const int AnswerCount = 10;

        int ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        Assert.AreEqual(QuestionCount, response.query.questions.Count);
        Assert.AreEqual(QName, response.query.questions[0].name.Name);

        Assert.AreEqual(AnswerCount, response.answers.Count);

        ptr = 52;
        var cnameLabel = DnsName.Read(buffer, ref ptr);

        Assert.AreEqual("audio-ak-spotify-com.akamaized.net", cnameLabel);

        int qPtr = 0;
        Assert.AreEqual(RecordTypes.CNAME, response.answers[qPtr].question.type);
        Assert.AreEqual(RecordClasses.IN, response.answers[qPtr++].question.@class);

        for (; qPtr < response.answers.Count; qPtr++)
        {
            Assert.AreEqual(RecordTypes.A, response.answers[qPtr].question.type);
            Assert.AreEqual(RecordClasses.IN, response.answers[qPtr].question.@class);
        }
    }

    private void Equal(Answer answer, string qName, RecordTypes qType, RecordClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        Assert.AreEqual(qName, answer.question.name.Name);
        Assert.AreEqual(qType, answer.question.type);
        Assert.AreEqual(qClass, answer.question.@class);
        Assert.AreEqual((uint)ttl, answer.ttl);
        Assert.AreEqual(rdLength, answer.data.length);
        Assert.AreEqual(rData.Length, answer.data.length);

        for (int i = 0; i < rData.Length; i++)
        {
            Assert.AreEqual(answer.data.data[i], rData[i]);
        }
    }

    private readonly byte[] _spotifyDnsRequest = [0x88, 0x5E, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0x61, 0x75, 0x64, 0x69, 0x6F, 0x2D, 0x61, 0x6B, 0x2D, 0x73, 0x70, 0x6F, 0x74, 0x69, 0x66, 0x79, 0x2D, 0x63, 0x6F, 0x6D, 0x09, 0x61, 0x6B, 0x61, 0x6D, 0x61, 0x69, 0x7A, 0x65, 0x64, 0x03, 0x6E, 0x65, 0x74, 0x00, 0x00, 0x01, 0x00, 0x01];
    private readonly byte[] _spotifyDnsResponse = [
        // MESSAGE START
        0x88, 0x5E, // Identification:      34910
        0x81, 0x80, // Flags:               1000000110000000
        0x00, 0x01, // Question Count:      1
        0x00, 0x0A, // Answers Count:       10
        0x00, 0x00, // Authorities Count:   0
        0x00, 0x00, // Additional Count:    0
        
        // QUESTION START
                    // QName
/*b12*/ 0x14,       // Label Length:        20
        // audio-ak-spotify-com
        0x61, 0x75, 0x64, 0x69, 0x6F, 0x2D, 0x61, 0x6B, 0x2D, 0x73, 0x70, 0x6F, 0x74, 0x69, 0x66, 0x79, 0x2D, 0x63, 0x6F, 0x6D,

        0x09,       // Label Length:        09
        // akamaized
        0x61, 0x6B, 0x61, 0x6D, 0x61, 0x69, 0x7A, 0x65, 0x64, 

/*b43*/ 0x03,       // Label Length:        03
        // net
        0x6E, 0x65, 0x74, 
        
        0x00,       // QName End

        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              A
        
        // ANSWER START
        0xC0, 0x0C, // Pointer, total value is 1100_0000_0000_1100 so 12
                    
/*b54*/ 0x00, 0x05, // QType:               CNAME
        0x00, 0x01, // QClass:              IN
/*b58*/ 0x00, 0x00, 0x00, 0x94, // TTL:     148

        0x00, 0x13, // Data Length:         19

                    // QName
/*b64*/ 0x04,       // Label Length:        4
        0x61, 0x32, 0x39, 0x37, //          a297

        0x04,       // Label Length:        4
        0x64, 0x73, 0x63, 0x63, //          dscc

        0x06,       // Label Lengh:         6
        // akamai
        0x61, 0x6B, 0x61, 0x6D, 0x61, 0x69, 
        
        0xC0, 0x2B, // Pointer, total value is 1100_0000_0010_1011 so 43

        // ANSWER START
/*b83*/ 0xC0, 0x40, // Pointer, total value is 1100_0000_0100_0000 so 64
        
        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20

        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0xCA, // Data:    20.200.0.202
        
        // ANSWER START
        0xC0, 0x40, // QName Pointer, total value is 1100_0000_0100_0000 so 64

        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20

        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0x9A, // Data:    20.200.0.154

        // ANSWER START
        0xC0, 0x40, // QName Pointer: 64
        
        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20

        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0xDA, // Data:    20.200.0.218
        
        // ANSWER START
        0xC0, 0x40, // QName Pointer: 64
        
        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20
        
        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0xD2, // Data:    20.200.0.210

        // ANSWER START
        0xC0, 0x40, // QName Pointer:       64
        
        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20

        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0xE1, // Data:    20.200.0.225
        
        // ANSWER START
        0xC0, 0x40, // QName Pointer:       64
        
        0x00, 0x01, // QType:               A
        0x00, 0x01, // QClass:              IN
        0x00, 0x00, 0x00, 0x14, // TTL:     20

        0x00, 0x04, // Data Length:         04
        0x18, 0xC8, 0x00, 0xDB, // Data:    20.200.0.219
        
        // ANSWER START
        0xC0, 0x40, // QName Pointer:       64

        0x00, 0x01, 
        0x00, 0x01, 
        0x00, 0x00, 0x00, 0x14, 
        
        0x00, 0x04, 
        0x18, 0xC8, 0x00, 0x8B, // Data:    20.200.0.139

        // ANSWER START
        0xC0, 0x40, 
        
        0x00, 0x01, 
        0x00, 0x01, 
        0x00, 0x00, 0x00, 0x14, 
        
        0x00, 0x04, 
        0x18, 0xC8, 0x00, 0x8A, // Data:    20.200.0.138
        
        // ANSWER START
        0xC0, 0x40, 
        
        0x00, 0x01, 
        0x00, 0x01, 
        0x00, 0x00, 0x00, 0x14,
        
        0x00, 0x04, 
        0x18, 0xC8, 0x00, 0x9B // Data:     20.200.0.155
    ];

    // 0x88,0x5E,
    // 0x81,0x80,
    // 0x00,0x01,
    // 0x00,0x0A,
    // 0x00,0x00,
    // 0x00,0x00,
    //
    // 0x14,
    // 0x61,0x75,0x64,0x69,0x6F,0x2D,0x61,0x6B,0x2D,0x73,0x70,0x6F,0x74,0x69,0x66,0x79,0x2D,0x63,0x6F,0x6D,
    //
    // 0x09,
    // 0x61,0x6B,0x61,0x6D,0x61,0x69,0x7A,0x65,0x64,
    //
    // 0x03,
    // 0x6E,0x65,0x74,
    //
    // 0x00,
    // 0x00,0x01,
    // 0x00,0x01,
    //
    // 0xC0,0x0C,
    //
    // 0x00,0x05,
    // 0x00,0x01,

    // 0x00,0x00,0x00,0xB7,
    //
    // 0x00,0x13,
    // 0x04,0x61,0x32,0x39,0x37,0x04,0x64,0x73,0x63,0x63,0x06,0x61,0x6B,0x61,0x6D,0x61,0x69,0xC0,0x2B,
    //
    // 0xC0,0x40,
    //
    // 0x00,0x01,
    // 0x00,0x01,
    // 0x00,0x00,0x00,0x14,
    //
    // 0x00,0x04,
    // 0x18,0xC8,0x00,0xD9,
    //
    // 0xC0,0x40,
    //
    // 0x00,0x01,
    // 0x00,0x01,
    // 0x00,0x00,0x00,0x14,
    //
    // 0x00,0x04,
    // 0x18,0xC8,0x00,0xE0,
    //
    // 0xC0,0x40,
    //
    // 0x00,0x01,
    // 0x00,0x01,
    // 0x00,0x00,0x00,0x14,
    //
    // 0x00,0x04,
    // 0x18,0xC8,0x00,0xC0,
    //
    // 0xC0,0x40,
    //
    // 0x00,0x01,
    // 0x00,0x01,
    //
    // 0x00,0x00,0x00,0x14,
    //
    // 0x00,0x04,
    // 0x18,0xC8,0x00,0xBB,
    //
    // 0xC0,0x40,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x14,0x00,0x04,0x18,0xC8,0x00,0xC2,0xC0,0x40,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x14,0x00,0x04,0x18,0xC8,0x00,0xD1,0xC0,0x40,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x14,0x00,0x04,0x18,0xC8,0x00,0xD3,0xC0,0x40,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x14,0x00,0x04,0x18,0xC8,0x00,0xC8,0xC0,0x40,0x00,0x01,0x00,0x01,0x00,0x00,0x00,0x14,0x00,0x04,0x18,0xC8,0x00,0xCA
}
