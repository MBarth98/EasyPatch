namespace Global.Format;

using Language.Extension;

public static partial class _
{

    public static byte[] ToBytes(this int int32)
    {
        byte[] data = new byte[4];
        data[0] = (byte)int32;
        data[1] = (byte)(int32 >> 8);
        data[2] = (byte)(int32 >> 16);
        data[3] = (byte)(int32 >> 24);
        return data;
    }

    public static byte[] ToBytes(this long int64)
    {
        byte[] data = new byte[8];
        data[0] = (byte)int64;
        data[1] = (byte)(int64 >> 8);
        data[2] = (byte)(int64 >> 16);
        data[3] = (byte)(int64 >> 24);
        data[4] = (byte)(int64 >> 32);
        data[5] = (byte)(int64 >> 40);
        data[6] = (byte)(int64 >> 48);
        data[7] = (byte)(int64 >> 56);
        return data;
    }

    public static int ToInt32(this Span<byte> data)
    {
        return data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
    }

    public static long ToInt64(this Span<byte> data)
    {
        return data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24) |
            (data[4] << 32) | (data[5] << 40) | (data[6] << 48) | (data[7] << 56);
    }

    public static byte[] ToBytes(this Header header)
    {
        var span = new Span<byte>(new byte[16]);

        Header.magic.ToBytes().CopyTo(span[0..4]);
        header.header_checksum.ToBytes().CopyTo(span[4..8]);
        Header.version.ToBytes().CopyTo(span[8..12]);
        header.length.ToBytes().CopyTo(span[12..16]);

        return span.ToArray();
    }

    public static byte[] ToBytes(this Entry header) 
    {
        var span = new List<byte>();
        
        span.AddRange(header.offset.ToBytes());

        span.AddRange(header.length.ToBytes());

        span.Add((byte)header.action);
        span.Add((byte)header.method);

        span.AddRange((IEnumerable<byte>) (header.data ?? new byte[0]));

        return span.ToArray();
    }

    public static Header ToHeader(this Span<byte> span)
    {
        var header = new Header();

        if (Header.magic != span[0..4].ToInt32())
        {
            throw new Exception("Invalid magic number");
        }

        if (Header.version != span[8..12].ToInt32())
        {
            throw new Exception("Invalid version number");
        }

        // validate checksum later...
        header.header_checksum = span[4..8].ToInt32();

        header.length = span[12..16].ToInt32();

        return header;
    }

    public static Entry ToEntry(this Span<byte> span)
    {
        if (span == null)
        {
            throw new ArgumentNullException(nameof(span));
        }

        switch((MethodType)span[9])
        {
            case MethodType.RAW:
                return new Entry() {
                    offset = span[0..4].ToInt32(),
                    length = span[4..8].ToInt32(),
                    action = (ActionType)span[8],
                    data = span[10..].ToArray()
            };
        };

        throw new Exception("Invalid method type");
    }

    public static int get_size_in_bytes(this Entry entry)
    {
        return sizeof(int) + sizeof(int) + sizeof(byte) + sizeof(byte) + entry.data.Length;
    }
}

