namespace Global.Patcher;


public enum Equality
{
    EQUAL,
    NOT_EQUAL,
};

public class ByteEquality
{
    public Equality kind;

    public byte? original;
    public byte? updated;
};

public class GroupEquality
{
    public Equality kind;

    public List<ByteEquality> bytes = new List<ByteEquality>();
};
 

public class Classifier
{
    public Classifier() {}

    public List<GroupEquality> ClassifyGroups(List<ByteEquality> bytes)
    {
        List<GroupEquality> groups = new List<GroupEquality>() {
            new GroupEquality() { kind = bytes[0].kind }
        };
        
        foreach (var elem in bytes)
        {
            if (elem.kind != groups.Last().kind)  
            {
                groups.Add(new GroupEquality() 
                {
                    kind = elem.kind,
                });
            }

            groups.Last().bytes.Add(elem);
        }

        return groups;
    }

    public List<ByteEquality> ClassifyBytes(Span<byte> lhs, Span<byte> rhs)
    {
        List<ByteEquality> tokens = new List<ByteEquality>();

        List<byte?> original = new List<byte?>(Array.ConvertAll<byte, byte?>(lhs.ToArray(), (x) => (byte?)x));
        List<byte?> updated  = new List<byte?>(Array.ConvertAll<byte, byte?>(rhs.ToArray(), (x) => (byte?)x));
        
        PadWithNull(ref original, ref updated);

        for (int i = 0; i < original.Count; i++)
        {
            tokens.Add(new ByteEquality() 
            { 
                kind = original[i] != updated[i] ? Equality.NOT_EQUAL : Equality.EQUAL, 
                original = original[i], 
                updated = updated[i] 
            });
        }

        return tokens;
    }

    public static void Print(List<GroupEquality> groups)
    {
        foreach (var group in groups)
        {
            Console.Write($"{group.kind} [");
            foreach (var elem in group.bytes)
            {
                Console.Write($"{{0x{(elem.original ?? 0):X2}, 0x{(elem.updated ?? 0):X2}}}");
            }

            Console.WriteLine("]");
        }
    }

    public static void Print(List<ByteEquality> bytes)
    {
        bytes.Aggregate(0, (acc, _byte) =>
        {
            Console.Write($"[{_byte.kind}, 0x{(_byte.original ?? 0):X2}, 0x{(_byte.updated ?? 0):X2}] ");
            return acc + 1;
        });

        Console.WriteLine();
    }

    private static void PadWithNull(ref List<byte?> lhs, ref List<byte?> rhs)
    {
        if (lhs.Count == rhs.Count)
            return;

        var diff = Math.Abs(lhs.Count - rhs.Count);

        if (lhs.Count > rhs.Count)
        {
            for (int i = 0; i < diff; i++)
            {
                rhs.Add(null);
            }

            return;
        }

        for (int i = 0; i < diff; i++)
        {
            lhs.Add(null);
        }
    }
}

