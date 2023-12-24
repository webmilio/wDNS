using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using wDNS.Common;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Knowledge;

public class HostFile : IQuestionable
{
    // TODO: This is an extremely bad placeholder. I want to write something with reflection that works on
    // SerializationOptions, that is able to figure out which line takes what parameters, etc.
    private static readonly Dictionary<string, Action<string[], SerializationOptions>> _options = new()
    {
        { 
            "ttl", 
            delegate (string[] split, SerializationOptions options)
            {
                options.DefaultTTL = int.Parse(split[1]);
            }
        }
    };

    public Dictionary<Question, List<Answer>> Answers { get; } = new Dictionary<Question, List<Answer>>(new GlobalEntryComparer());

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
                
                var answers = hosts.Answers.GetOrProvide(answer.Question, _ => new());

                answers.Add(answer);
            }
        }

        return hosts;
    }

    private static Answer ParseLine(string line, SerializationOptions options)
    {
        var s = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var address = NetworkHelpers.ParseIPAddress(s[0]);

        var qType = address.AddressFamily == AddressFamily.InterNetworkV6 ? QTypes.AAAA : QTypes.A;

        var question = ParseQuestion(s[1], qType, options);
        return ParseAnswer(address, qType, question, options);
    }

    private static Question ParseQuestion(string segment, QTypes qType, SerializationOptions options)
    {
        var dnsName = new DnsName(segment);
        return new Question
        {
            QName = dnsName,

            // TODO: Hardcoding things like this is not very cool, figure out how to change that later.
            QType = qType,

            // TODO: Same as above, but I know nothing of the other classes.
            QClass = QClasses.IN
        };
    }

    private static Answer ParseAnswer(IPAddress address, QTypes qType, Question question, SerializationOptions options)
    {
        var bytes = address.GetAddressBytes();

        return new Answer
        {
            Question = question,
            TTL = (uint)options.DefaultTTL,
            Data = new(qType, (ushort)bytes.Length, bytes, address)
        };
    }

    public bool TryAnswer(Question question, out IList<Answer>? answers)
    {
        var result = Answers.TryGetValue(question, out var mAnswers);
        answers = mAnswers;

        return result;
    }

    public class SerializationOptions
    {
        public static SerializationOptions Default => new();

        public char Comment { get; set; } = '#';
        public char Configuration { get; set; } = '@';

        public int DefaultTTL { get; set; } = (int)TimeSpan.FromMinutes(5).TotalSeconds;
    }

    public class GlobalEntryComparer : IEqualityComparer<Question>
    {
        public bool Equals(Question? x, Question? y)
        {
            return x != null && y != null && x.QName.Equals(y.QName);
        }

        public int GetHashCode([DisallowNull] Question obj)
        {
            return obj.QName.GetHashCode();
        }
    }
}
