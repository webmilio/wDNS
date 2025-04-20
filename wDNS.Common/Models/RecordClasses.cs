namespace wDNS.Common.Models;

public enum RecordClasses : ushort
{
    /// <summary>Internet (default class)</summary>
    IN = 1,

    /// <summary>CSNET class (obsolete)</summary>
    CS = 2,

    /// <summary>CHAOS class (e.g., DNAME queries)</summary>
    CH = 3,

    /// <summary>Hesiod class</summary>
    HS = 4,

    /// <summary>Wildcard for any class (*)</summary>
    ANY = 255
}
