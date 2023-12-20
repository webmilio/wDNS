namespace wDNS.Common;

/// <summary>
/// Needs a Query, Opcode, Authoritative, Truncation, RecursionDesired, RecursionAvailable, ResponseCode
/// </summary>
[Flags]
public enum MessageFlags : ushort
{
    Query_Query = 0b0000000000000000,
    Query_Response = 0b1000000000000000,

    Opcode_Standard = 0b0000000000000000,
    Opcode_Inverse = 0b0000100000000000,
    Opcode_ServerStatus = 0b0001000000000000,

    Authoritative_NonAuthoritative = 0b0000000000000000,
    Authoritative_Authoritative = 0b0000010000000000,

    Truncation_Entire = 0b0000000000000000,
    Truncation_Truncated = 0b0000001000000000,

    RecursionDesired_NotDesired = 0b0000000000000000,
    RecursionDesired_Desired = 0b0000000100000000,

    RecursionAvailable_Unsupported = 0b0000000000000000,
    RecursionAvailable_Supported = 0b0000000010000000,

    // Z doesn't exist, but 1 << 4, 3 bits long
    //                                  0b0000000001110000

    ResponseCode_NoError = 0b0000000000000000,
    ResponseCode_FormatError = 0b0000000000000001,
    ResponseCode_ServerFailure = 0b0000000000000010,
    ResponseCode_NameError = 0b0000000000000011,
    ResponseCode_NotImplemented = 0b0000000000000100,
    ResponseCode_Refused = 0b0000000000000101
}

public enum QTypes : ushort
{
    A = 1,
    NS = 2,
    CNAME = 5,
    SOA = 6,
    MB = 7,
    MG = 8,
    MR = 9,
    NULL = 10,
    WKS = 11,
    PTR = 12,
    HINFO = 13,
    MINFO = 14,
    MX = 15,
    TXT = 16,
    AAAA = 28,
    SRV = 33
}

public enum QClasses : ushort
{
    IN = 1,
    CS = 2,
    CH = 3,
    HS = 4
}