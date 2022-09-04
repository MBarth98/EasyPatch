namespace Language.Extension;


public static partial class _
{
    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        for (int i = range.Start.Value; i < range.End.Value; i++)
        {
            yield return i;
        }
    }

    public static byte _next(this ref Span<byte> data)
    {
        var value = data[0];
        data = data.Slice(1);
        return value;
    }

    public static byte? _next_if(this ref Span<byte> data, byte expected)
    {
        var value = data[0];
        if (value != expected)
            return null;
        
        data = data.Slice(1);
        return value;
    }

    public static byte _peek(this ref Span<byte> data)
    {
        return data[0];
    }
}