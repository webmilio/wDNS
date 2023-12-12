using wDNS.Models;

namespace wDNS;

public class Constants
{
    // Math goes roughly ID x2, Flags x2, 4x counts (question, etc.) x2 (so x8) QTYPE x2, QCLASS x2
    // 2 + 2 + 8 + 2 + 2 = 16
    public const int UdpPacketLengthWithoutQNAMEInBytes = sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt16) * 4 + sizeof(QTypes) + sizeof(QClasses);

    public const int UdpPacketMaxLength = 255;
}
