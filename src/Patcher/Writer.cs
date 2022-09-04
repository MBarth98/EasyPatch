namespace Global.Patcher;

using System.Linq;
using Global.Format;

using Global.Patcher.Error;

public class Writer
{

    private static Span<byte> Read(string filepath)
    {
        if (File.Exists(filepath))
        {
            return File.ReadAllBytes(filepath);
        }

        return null;
    }

    public static SectionResult CreateSection(string target_file, Span<byte> original, Span<byte> updated)
    {
        Classifier classifier = new Classifier();
        var clasifications = classifier.ClassifyBytes(original, updated);

        var groups = classifier.ClassifyGroups(clasifications);

        List<Entry> entries = new List<Entry>();

        int currentOffset = 0;

        foreach (var group in groups)
        {

            if (group.kind == Equality.NOT_EQUAL)
            {
                entries.Add(new Entry()
                {
                    offset = currentOffset,
                    length = group.bytes.Count,
                    action = Format.ActionType.REPLACE,
                    method = Format.MethodType.RAW,
                    data = group.bytes.Select(x => x.updated ?? 0).ToArray()
                });
            }

            currentOffset += (group.bytes.Count);

        }

        return SectionResult.OK(new Section()
        {
            rel_path = target_file,
            version = 1,
            entries = entries,
            section_length = entries.Sum(x => x.get_size_in_bytes())
        });
    }

     
    private static List<(string filename, string original_path, string updated_path)> EnumerateFiles(string original_root, string updated_root)
    {
        List<(string filename, string original_path, string updated_path)> 
        files = new List<(string, string, string)>();

        var original = Directory.EnumerateFiles(original_root, "*", SearchOption.AllDirectories);
        var updated = Directory.EnumerateFiles(updated_root, "*", SearchOption.AllDirectories);

        var Combine = original.Zip(updated, (string? x, string? y) => (x ?? "REMOVED", y ?? "ADDED"));

        foreach (var file in Combine)
        {   
            var filename = file.Item1.Substring(original_root.Length);
            files.Add((filename, file.Item1, file.Item2));
        }

        return files;
    }

    public static PatchResult CreatePatch(string original, string updated)
    {
        var files = EnumerateFiles(original, updated);

        Classifier classifier = new Classifier();

        List<SectionResult> sections = new List<SectionResult>();

        foreach (var file in files)
        {
            var original_bytes = Read(file.original_path);
            var updated_bytes = Read(file.updated_path);

            if (original_bytes == null || updated_bytes == null)
            {
                return PatchResult.Error("Could not read file", file.filename);
            }

            var section = CreateSection(file.filename, original_bytes, updated_bytes);

            if (!section)
            {
                return PatchResult.Error("Could not create section. reason: " + section.message, file.filename);
            }

            sections.Add(section);
        }

        List<Section> valid_sections =  new List<Section>();
        
        sections.ForEach((x) => { 
            valid_sections.Add(x.result ?? throw new Exception("SectionResult.success is true but result is null"));
        });

        return PatchResult.OK(new Patch(){
            sections = valid_sections
        });

    }
}
