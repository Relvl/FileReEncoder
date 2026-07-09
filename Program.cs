using System.Text;
using UtfUnknown;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var opts = Args(args);
var fix = opts.ContainsKey("fix");
var target = Encoding.GetEncoding(opts.GetValueOrDefault("target", "utf-8"));
var exts = opts.GetValueOrDefault("ext", "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(x => "." + x.TrimStart('.').ToLowerInvariant())
    .ToHashSet();

if (exts.Count == 0)
{
    Console.WriteLine("Usage: -ext:sql,txt -target:windows-1251 [-fix]");
    return;
}

foreach (var file in Directory.EnumerateFiles(".", "*", SearchOption.AllDirectories)
             .Where(f => exts.Contains(Path.GetExtension(f).ToLowerInvariant())))
{
    var bytes = File.ReadAllBytes(file);
    if (bytes.Length == 0 || IsAscii(bytes))
        continue;

    var detected = CharsetDetector.DetectFromBytes(bytes).Detected;
    var source = detected?.Encoding;

    if (source == null)
    {
        Console.WriteLine($"{Path.GetRelativePath(".", file)}: unknown");
        continue;
    }

    if (Same(source, target))
        continue;

    Console.WriteLine($"{Path.GetRelativePath(".", file)}: {source.WebName} -> {target.WebName} ({detected!.Confidence:P0})");

    if (fix)
    {
        var text = source.GetString(bytes);
        File.WriteAllBytes(file, target.GetBytes(text));
    }
}

static Dictionary<string, string> Args(string[] args) =>
    args.Where(a => a.StartsWith('-'))
        .Select(a => a[1..].Split(':', 2))
        .ToDictionary(p => p[0].ToLowerInvariant(), p => p.Length > 1 ? p[1] : "true");

static bool Same(Encoding a, Encoding b) =>
    a.CodePage == b.CodePage;

static bool IsAscii(byte[] bytes) =>
    bytes.All(b => b < 128);