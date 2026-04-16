using System.Globalization;
using System.Security.Cryptography;

namespace Genderize.Api.Helpers;

public static class Uuid7Helper
{
    public static Guid Create()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        bytes[0] = (byte)(unixTimeMilliseconds >> 40);
        bytes[1] = (byte)(unixTimeMilliseconds >> 32);
        bytes[2] = (byte)(unixTimeMilliseconds >> 24);
        bytes[3] = (byte)(unixTimeMilliseconds >> 16);
        bytes[4] = (byte)(unixTimeMilliseconds >> 8);
        bytes[5] = (byte)unixTimeMilliseconds;
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        var uuidText = string.Create(CultureInfo.InvariantCulture, $"{bytes[0]:x2}{bytes[1]:x2}{bytes[2]:x2}{bytes[3]:x2}-{bytes[4]:x2}{bytes[5]:x2}-{bytes[6]:x2}{bytes[7]:x2}-{bytes[8]:x2}{bytes[9]:x2}-{bytes[10]:x2}{bytes[11]:x2}{bytes[12]:x2}{bytes[13]:x2}{bytes[14]:x2}{bytes[15]:x2}");
        return Guid.Parse(uuidText);
    }
}
