using System.Text.RegularExpressions;

namespace wDNS.Common.Models;

public class RegexDnsName : DnsName
{
    private Regex _regex;

    public RegexDnsName(string name) : base(name)
    {
        _regex = new Regex(name);
    }

    public override bool Match(string name)
    {
        return _regex.IsMatch(name);
    }

    public override bool Match(byte[] data)
    {
        throw new NotSupportedException("RegexDnsNames do not support byte array matching.");
    }

    public override bool Equals(object? obj)
    {
        return obj is IDnsName name && Match(name.Name);
    }
}
