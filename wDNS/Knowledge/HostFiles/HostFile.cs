﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using wDNS.Common;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Knowledge.HostFiles;

public class HostFile : IQuestionable
{
    // TODO: This is an extremely bad placeholder. I want to write something with reflection that works on
    // SerializationOptions, that is able to figure out which line takes what parameters, etc.
    private static readonly Dictionary<string, Action<string[], SerializationOptions>> _options = new()
    {
        {
            "ttl", delegate (string[] split, SerializationOptions options) {
                options.DefaultTTL = int.Parse(split[1]);
            }
        },
        {
            "regex", delegate (string[] split, SerializationOptions options) {
                options.RegexMatching = true;
            }
        }
    };

    public Dictionary<Question, List<Answer>> Answers { get; } = new Dictionary<Question, List<Answer>>(new GlobalEntryComparer());
    
    private delegate bool TryAnswerDelegate(Question question, QuestionResult result);
    private TryAnswerDelegate _tryAnswerMethod;

    public bool TryAnswer(Question question, QuestionResult result)
    {
        return _tryAnswerMethod(question, result);
    }

    private bool TryAnswerNormal(Question question, QuestionResult result)
    {
        if (Answers.TryGetValue(question, out var answers))
        {
            result.Answers.AddRange(answers);
            result.Flags |= MessageFlags.Authoritative_Authoritative;

            return true;
        }

        return false;
    }

    private bool TryAnswerRegex(Question question, QuestionResult result)
    {
        try
        {
            var pair = Answers.First(k => k.Key.name.Match(question.name.Name));

            result.Answers.AddRange(pair.Value);
            result.Flags |= MessageFlags.Authoritative_Authoritative;

            return true;
        }
        catch (Exception)
        {
            return false; // Try-Catches are slow but I can't compare a struct to 'default' so it'll do.
        }
    }

    public static Task<HostFile> Read(TextReader reader) => Read(reader, SerializationOptions.Default);

    public static async Task<HostFile> Read(TextReader reader, SerializationOptions serializationOptions)
    {
        var hosts = new HostFile();

        while (reader.Peek() > 0)
        {
            var line = (await reader.ReadLineAsync())!;
            line = line.TrimStart('\t', ' ');

            if (line.StartsWith(serializationOptions.Comment) || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            else if (line.StartsWith(serializationOptions.Configuration))
            {
                line = line.Remove(0, 1);
                var s = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (_options.TryGetValue(s[0], out var action))
                {
                    action(s, serializationOptions);
                }
            }
            else
            {
                Answer answer;
                try
                {
                    answer = ParseLine(line, serializationOptions);
                }
                catch
                {
                    continue;
                }

                var answers = hosts.Answers.GetOrProvide(answer.question, _ => new());

                answers.Add(answer);
            }
        }

        hosts._tryAnswerMethod = serializationOptions.RegexMatching ? hosts.TryAnswerRegex : hosts.TryAnswerNormal;
        return hosts;
    }

    private static Answer ParseLine(string line, SerializationOptions options)
    {
        var s = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var address = NetworkHelpers.ParseIPAddress(s[0]);

        var qType = address.AddressFamily == AddressFamily.InterNetworkV6 ? RecordTypes.AAAA : RecordTypes.A;

        var question = ParseQuestion(s[1..], qType, options);
        return ParseAnswer(address, qType, question, options);
    }

    private static Question ParseQuestion(string[] segment, RecordTypes qType, SerializationOptions options)
    {
        IDnsName dnsName;

        if (options.RegexMatching && segment[0].StartsWith('/'))
        {
            dnsName = new RegexDnsName(string.Join(' ', segment));
        }
        else
        {
            dnsName = new DnsName(segment[0]);
        }
        
        return new Question
        {
            name = dnsName,

            // TODO: Hardcoding things like this is not very cool, figure out how to change that later.
            type = qType,

            // TODO: Same as above, but I know nothing of the other classes.
            @class = RecordClasses.IN
        };
    }

    private static Answer ParseAnswer(IPAddress address, RecordTypes qType, Question question, SerializationOptions options)
    {
        var bytes = address.GetAddressBytes();

        var data = new AnswerData(qType, (ushort)bytes.Length, bytes, address);
        return new Answer(question, data, (uint)options.DefaultTTL);
    }

    public class SerializationOptions
    {
        public static SerializationOptions Default => new();

        public char Comment { get; set; } = '#';
        public char Configuration { get; set; } = '@';

        public int DefaultTTL { get; set; } = (int)TimeSpan.FromMinutes(5).TotalSeconds;

        public bool RegexMatching { get; set; } = false;
    }

    public class GlobalEntryComparer : IEqualityComparer<Question>
    {
        public bool Equals(Question x, Question y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] Question obj)
        {
            return obj.name.GetHashCode();
        }
    }
}
