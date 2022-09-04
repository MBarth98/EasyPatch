namespace Global.Format;

public struct Patch
{
    public Header header { get; set; }
    public List<Section> sections { get; set; }
}

public struct Header
{
   // magic number for file format
   public const int magic = Global.Constants.magic;

   // verify that the file has the expected number of bytes
   public int header_checksum { get; set; }

   // version of the file format
   public const int version = Global.Constants.version;

   public int length { get; set; }
};

public struct Section
{
    // relative location of the file + name in the root directory // the section context (FILE*)
    public string rel_path { get; set; }

    // used to apply the patch in the correct order (in case of merged patches) 
    public int version { get; set; }

    public int section_checksum { get; set; }

    public int section_length { get; set; }

    public List<Entry> entries { get; set; }

    public override string ToString()
    {
        return $@"{{
    rel_path: {rel_path}
    version: {version}
    section_checksum: {section_checksum}
    section_length: {section_length}
    entries: {entries.GetRange(1, entries.Count - 1).Aggregate($"{entries[0]}", (a, b) => $"{a}, {b}")}
}}";
    }
}


// bit pattern: reduce errors caused by random bit flipping
public enum ActionType : byte
{
    // target_begin_offset: offset to the start of the target
    // run-length ignore by the (largest of either target_length or source_length) amount of bytes
    IGNORE  = (byte)0b0000_0000,

    // target_begin_offset: offset to the start of the target
    // target_length: the amount of bytes to be deleted
    // source_length and data: not used
    DELETE  = (byte)0b1010_1010,

    // target_begin_offset: offset to the start of the target
    // target_length and source_length: must be the same length
    REPLACE = (byte)0b0101_0101,

    // target_begin_offset: offset to the start of the target
    // target_length does not matter for this type
    // source_length is the length of the data to be inserted
    INSERT  = (byte)0b1001_0110
}

public enum MethodType : byte
{
    RAW
}

public class Entry
{
    public int offset { get; set; }
    public int length { get; set; }
    public ActionType action { get; set; }
    public MethodType method { get; set; }

    public byte[] data { get; set; } = {0};

    public override string ToString()
    {
        return $@"[
        offset: {offset}
        length: {length}
        action: {action}
        method: {method}
        data: {System.Text.Encoding.Default.GetString(data)}
    ]";
    }

};
