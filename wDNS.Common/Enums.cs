using System;
using System.Runtime.CompilerServices;

namespace wDNS.Common;

/// <summary>
/// Needs a Query, Opcode, Authoritative, Truncation, RecursionDesired, RecursionAvailable, ResponseCode
/// </summary>
[Flags]
public enum MessageFlags : ushort
{
    Query_Query = 0b0_000000000000000,
    Query_Response = 0b1_000000000000000,

    Opcode_Standard = 0b0_0000_000_00000000,
    Opcode_Inverse = 0b0_0001_000_00000000,
    Opcode_ServerStatus = 0b0_0010_00000000000,

    Authoritative_NonAuthoritative = 0b00000_0_0000000000,
    Authoritative_Authoritative = 0b00000_1_0000000000,

    Truncation_Entire = 0b000000_0_000000000,
    Truncation_Truncated = 0b000000_1_000000000,

    RecursionDesired_NotDesired = 0b0000000_0_00000000,
    RecursionDesired_Desired = 0b0000000_1_00000000,

    RecursionAvailable_Unsupported = 0b00000000_0_0000000,
    RecursionAvailable_Supported = 0b00000000_1_0000000,

    // Z doesn't exist, but 1 << 4, 3 bits long
    //                                  0b0000000001110000

    ResponseCode_NoError = 0b000000000000_0000,
    ResponseCode_FormatError = 0b000000000000_0001,
    ResponseCode_ServerFailure = 0b000000000000_0010,
    ResponseCode_NameError = 0b000000000000_0011,
    ResponseCode_NotImplemented = 0b000000000000_0100,
    ResponseCode_Refused = 0b000000000000_0101,

    ResponseCode_NameExistsWhenItShouldnt = 0b000000000000_0110,
    ResponseCode_RRSetExistsWhenitShouldnt = 0b000000000000_0111,
    ResponseCode_RRSetDoesntExistWhenitShould = 0b000000000000_1000,
    ResponseCode_NotAuthenticated = 0b000000000000_1001,
    ResponseCode_NotInZone = 0b000000000000_1010
}

public class MessageFlagsHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetFlag(ref MessageFlags flags, MessageFlags flag, bool value)
    {
        SetFlag(ref flags, flag, ~flag, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetFlag(ref MessageFlags flags, MessageFlags trueMask, MessageFlags falseMask, bool value)
    {
        flags = value ? (flags | trueMask) : flags & falseMask;
    }
}

public enum RecordClasses : ushort
{
    IN = 1,
    CS = 2,
    CH = 3,
    HS = 4
}