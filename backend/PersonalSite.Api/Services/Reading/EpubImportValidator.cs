using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;

namespace PersonalSite.Api.Services.Reading;

public record EpubMetadata(string Title, string Author, string Language);

// Fail-closed profile, not a general-purpose sanitizer. Accepted bytes are never rewritten:
// CFI refers to an immutable validated publication. Unsupported active content is rejected.
public sealed class EpubImportValidator(IOptions<ReadingLibraryOptions> options)
{
    private readonly ReadingLibraryOptions _limits = options.Value;
    private static readonly XNamespace Opf = "http://www.idpf.org/2007/opf";
    private static readonly XNamespace Dc = "http://purl.org/dc/elements/1.1/";
    private static readonly XNamespace Html = "http://www.w3.org/1999/xhtml";
    private static readonly HashSet<string> HtmlElements = new(StringComparer.Ordinal) {
        "html", "head", "title", "meta", "link", "style", "body", "div", "span", "p",
        "h1", "h2", "h3", "h4", "h5", "h6", "a", "img", "br", "hr", "em", "strong",
        "b", "i", "u", "s", "small", "sup", "sub", "blockquote", "pre", "code", "ul", "ol",
        "li", "dl", "dt", "dd", "table", "thead", "tbody", "tfoot", "tr", "th", "td", "caption",
        "colgroup", "col", "section", "article", "nav", "aside", "header", "footer", "main",
        "figure", "figcaption", "abbr", "cite", "q", "mark", "ruby", "rt", "rp", "wbr"
    };
    private static readonly HashSet<string> HtmlAttributes = new(StringComparer.Ordinal) {
        "id", "class", "title", "lang", "dir", "style", "href", "src", "alt", "width", "height",
        "rel", "type", "charset", "name", "content", "colspan", "rowspan", "scope", "start", "value"
    };
    private static readonly HashSet<string> CssProperties = new(StringComparer.OrdinalIgnoreCase) {
        "color", "background-color", "font-family", "font-size", "font-style", "font-weight",
        "font-variant", "line-height", "text-align", "text-indent", "text-decoration", "text-transform",
        "letter-spacing", "word-spacing", "white-space", "vertical-align", "text-justify", "display", "width", "height",
        "max-width", "max-height", "min-width", "min-height", "margin", "margin-top", "margin-bottom",
        "margin-left", "margin-right", "padding", "padding-top", "padding-bottom", "padding-left",
        "padding-right", "border", "border-top", "border-bottom", "border-left", "border-right",
        "border-width", "border-style", "border-color", "border-collapse", "border-spacing",
        "page-break-before", "page-break-after", "page-break-inside", "break-before", "break-after",
        "break-inside", "orphans", "widows", "list-style-type", "list-style-position"
    };

    public EpubMetadata Validate(Stream stream)
    {
        try { return ValidateArchive(stream); }
        catch (ReadingLibraryException) { throw; }
        catch (Exception ex) when (ex is InvalidDataException or XmlException or ArgumentException or DecoderFallbackException)
        { throw Bad("invalid_epub", "EPUB archive or XML is invalid."); }
    }

    private EpubMetadata ValidateArchive(Stream stream)
    {
        if (_limits.MaxEntries <= 0 || _limits.MaxEntryBytes <= 0 || _limits.MaxExpandedBytes <= 0
            || _limits.MaxCompressionRatio <= 0) throw new InvalidOperationException("Invalid EPUB safety limits.");
        stream.Position = 0;
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        if (zip.Entries.Count == 0 || zip.Entries.Count > _limits.MaxEntries)
            throw Bad("archive_limits", "Too many archive entries.");
        var entries = new Dictionary<string, ZipArchiveEntry>(StringComparer.Ordinal);
        long total = 0;
        foreach (var entry in zip.Entries)
        {
            // Never extract ZIP paths. Reject aliases, traversal, symlinks and duplicate names anyway.
            var path = entry.FullName;
            if (path.Length > 1024 || path.StartsWith('/') || path.Contains('\\') || path.Contains(':')
                || path.Contains('%') || path.Any(char.IsControl)
                || path.Split('/').Any(s => s is "." or "..")
                || ((entry.ExternalAttributes >> 16) & 0xF000) == 0xA000)
                throw Bad("unsafe_archive_path", "Unsafe EPUB archive path.");
            if (path.EndsWith('/')) { if (entry.Length != 0) throw Bad("invalid_epub", "Invalid directory."); continue; }
            if (path.Length == 0 || path.Contains("//") || !entries.TryAdd(path, entry))
                throw Bad("unsafe_archive_path", "Duplicate or ambiguous EPUB archive path.");
            if (entry.Length > _limits.MaxEntryBytes || entry.Length > _limits.MaxExpandedBytes - total
                || entry.Length > Math.Max(1, entry.CompressedLength) * (double)_limits.MaxCompressionRatio)
                throw Bad("archive_limits", "EPUB exceeds decompression safety limits.");
            total += entry.Length;
        }
        if (!entries.TryGetValue("mimetype", out var mime) || zip.Entries[0] != mime
            || Text(mime) != "application/epub+zip")
            throw Bad("invalid_epub", "Missing or invalid EPUB mimetype.");
        var container = Xml(Required(entries, "META-INF/container.xml"));
        XNamespace containerNs = "urn:oasis:names:tc:opendocument:xmlns:container";
        if (container.Root?.Name != containerNs + "container") throw Bad("invalid_epub", "Invalid container.");
        var roots = container.Descendants(containerNs + "rootfile").ToList();
        if (roots.Count != 1 || (string?)roots[0].Attribute("media-type") != "application/oebps-package+xml")
            throw Bad("unsupported_epub", "Only a single EPUB rendition is supported.");
        var packagePath = Resolve("", (string?)roots[0].Attribute("full-path") ?? "");
        var package = Xml(Required(entries, packagePath));
        if (package.Root?.Name != Opf + "package" || !new[] { "2.0", "3.0" }.Contains((string?)package.Root.Attribute("version")))
            throw Bad("unsupported_epub", "Only EPUB 2/3 packages are supported.");
        ValidatePackageXml(package);
        foreach (var reference in package.Descendants(Opf + "reference"))
            Required(entries, Resolve(packagePath, (string?)reference.Attribute("href") ?? ""));
        if (package.Descendants(Opf + "meta").Any(m => (string?)m.Attribute("property") == "rendition:layout"
            && m.Value.Trim() == "pre-paginated")) throw Bad("unsupported_epub", "Fixed layout is not supported.");
        var manifest = package.Root.Element(Opf + "manifest")?.Elements(Opf + "item").ToList();
        var spine = package.Root.Element(Opf + "spine");
        if (manifest == null || manifest.Count == 0 || spine == null) throw Bad("invalid_epub", "Missing manifest or spine.");
        var ids = new Dictionary<string, string>(StringComparer.Ordinal);
        var types = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in manifest)
        {
            var id = (string?)item.Attribute("id") ?? "";
            var type = (string?)item.Attribute("media-type") ?? "";
            var path = Resolve(packagePath, (string?)item.Attribute("href") ?? "");
            if (id.Length == 0 || !ids.TryAdd(id, path) || !types.TryAdd(path, type))
                throw Bad("invalid_epub", "Duplicate manifest item.");
            Required(entries, path);
            if (((string?)item.Attribute("properties") ?? "").Split(' ').Any(p => p is "scripted" or "remote-resources"))
                throw Bad("unsafe_content", "Scripted and remote resources are not supported.");
        }
        if (spine.Attribute("toc") is { } toc && (!ids.TryGetValue(toc.Value, out var ncxPath)
            || types[ncxPath] != "application/x-dtbncx+xml"))
            throw Bad("invalid_epub", "Invalid NCX reference.");
        var refs = spine.Elements(Opf + "itemref").ToList();
        if (refs.Count == 0 || refs.All(r => (string?)r.Attribute("linear") == "no"))
            throw Bad("invalid_epub", "No readable spine.");
        foreach (var item in refs)
            if (!ids.TryGetValue((string?)item.Attribute("idref") ?? "", out var path) || types[path] != "application/xhtml+xml")
                throw Bad("invalid_epub", "Invalid spine reference.");

        // Every file must have a known role; DRM, SVG, scripts, fonts and stray files fail closed.
        foreach (var (path, entry) in entries)
        {
            if (path == "mimetype" || path == "META-INF/container.xml" || path == packagePath) continue;
            if (!types.TryGetValue(path, out var type)) throw Bad("unsupported_epub", "Unmanifested resources are not supported.");
            switch (type)
            {
                case "application/xhtml+xml": ValidateHtml(Xml(entry), path, entries); break;
                case "application/x-dtbncx+xml": ValidateNcx(Xml(entry), path, entries); break;
                case "text/css": ValidateCss(Text(entry), declarationsOnly: false); break;
                case "image/png": case "image/jpeg": case "image/gif": ValidateImage(Bytes(entry), type); break;
                default: throw Bad("unsupported_epub", "Unsupported resource type: " + type);
            }
        }
        var title = package.Descendants(Dc + "title").FirstOrDefault()?.Value.Trim() ?? "";
        var author = string.Join(", ", package.Descendants(Dc + "creator").Select(x => x.Value.Trim()));
        var language = package.Descendants(Dc + "language").FirstOrDefault()?.Value.Trim() ?? "";
        if (title.Length == 0 || title.Length > 512 || author.Length > 512 || language.Length > 64)
            throw Bad("invalid_metadata", "EPUB metadata is missing or too long.");
        return new(title, author, language);
    }

    public (byte[] Bytes, string ContentType)? ReadCover(Stream stream)
    {
        // Reuse bounded validation; never serve unchecked archive resources.
        Validate(stream);
        stream.Position = 0;
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        var entries = zip.Entries.Where(e => !e.FullName.EndsWith('/')).ToDictionary(e => e.FullName, StringComparer.Ordinal);
        XNamespace containerNs = "urn:oasis:names:tc:opendocument:xmlns:container";
        var container = Xml(Required(entries, "META-INF/container.xml"));
        var packagePath = Resolve("", (string)container.Descendants(containerNs + "rootfile").Single().Attribute("full-path")!);
        var package = Xml(Required(entries, packagePath));
        var items = package.Descendants(Opf + "item").ToList();
        var legacyId = package.Descendants(Opf + "meta").FirstOrDefault(m => (string?)m.Attribute("name") == "cover")?.Attribute("content")?.Value;
        var cover = items.FirstOrDefault(i => ((string?)i.Attribute("properties") ?? "").Split(' ').Contains("cover-image"))
            ?? items.FirstOrDefault(i => legacyId != null && (string?)i.Attribute("id") == legacyId);
        var type = (string?)cover?.Attribute("media-type");
        if (cover == null || type is not ("image/png" or "image/jpeg" or "image/gif")) return null;
        return (Bytes(Required(entries, Resolve(packagePath, (string)cover.Attribute("href")!))), type);
    }

    private byte[] Bytes(ZipArchiveEntry entry)
    {
        using var input = entry.Open();
        using var output = new MemoryStream();
        var buffer = new byte[81920];
        int read;
        while ((read = input.Read(buffer)) > 0)
        {
            if (output.Length + read > _limits.MaxEntryBytes || output.Length + read > entry.Length)
                throw Bad("archive_limits", "Expanded entry exceeds its declared size.");
            output.Write(buffer, 0, read);
        }
        if (output.Length != entry.Length) throw Bad("invalid_epub", "Truncated EPUB entry.");
        return output.ToArray();
    }

    private string Text(ZipArchiveEntry entry) => new UTF8Encoding(false, true).GetString(Bytes(entry));
    private XDocument Xml(ZipArchiveEntry entry)
    {
        using var input = new MemoryStream(Bytes(entry));
        using var reader = XmlReader.Create(input, new XmlReaderSettings {
            DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
            MaxCharactersInDocument = _limits.MaxEntryBytes, MaxCharactersFromEntities = 0
        });
        var document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        if (document.DescendantNodes().OfType<XProcessingInstruction>().Any())
            throw Bad("unsafe_content", "XML processing instructions are not supported.");
        if (document.Descendants().Any(e => e.Attributes().Any(a => a.Name == XNamespace.Xml + "base")))
            throw Bad("unsafe_content", "XML base is not supported.");
        return document;
    }

    private static ZipArchiveEntry Required(Dictionary<string, ZipArchiveEntry> entries, string path) =>
        entries.TryGetValue(path, out var entry) ? entry : throw Bad("invalid_epub", "Missing EPUB resource.");

    private static string Resolve(string source, string reference)
    {
        if (string.IsNullOrWhiteSpace(reference) || reference.Contains('\\') || reference.Contains(':')
            || reference.StartsWith('/') || reference.Contains('%') || reference.Contains('?') || reference.Any(char.IsControl))
            throw Bad("unsafe_content", "External or ambiguous resource reference.");
        var target = reference.Split('#')[0];
        if (target.Length == 0) return source;
        var parts = (source.Contains('/') ? source[..(source.LastIndexOf('/') + 1)] : "").Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (var segment in target.Split('/'))
        {
            if (segment == ".") continue;
            if (segment == "..") {
                if (parts.Count == 0) throw Bad("unsafe_content", "Resource escapes publication.");
                parts.RemoveAt(parts.Count - 1);
            }
            else if (segment.Length == 0) throw Bad("unsafe_content", "Ambiguous resource reference.");
            else parts.Add(segment);
        }
        return string.Join('/', parts);
    }

    private static void ValidatePackageXml(XDocument document)
    {
        var names = new HashSet<string> { "package", "metadata", "manifest", "item", "spine", "itemref", "meta", "guide", "reference", "bindings" };
        foreach (var element in document.Descendants())
        {
            if (element.Name.Namespace == Dc && !element.HasElements) continue;
            if (element.Name.Namespace != Opf || !names.Contains(element.Name.LocalName) || element.Name.LocalName == "bindings")
                throw Bad("unsafe_content", "Unsupported package markup.");
        }
    }

    private void ValidateHtml(XDocument document, string path, Dictionary<string, ZipArchiveEntry> entries)
    {
        if (document.Root?.Name != Html + "html") throw Bad("invalid_epub", "Content must be XHTML.");
        foreach (var element in document.Descendants())
        {
            if (element.Name.Namespace == "http://www.w3.org/2000/svg")
            {
                ValidateSvgImageWrapper(element, path, entries);
                continue;
            }
            if (element.Name.Namespace != Html || !HtmlElements.Contains(element.Name.LocalName))
                throw Bad("unsafe_content", "Unsupported XHTML element.");
            foreach (var attr in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
            {
                if (attr.Name == XNamespace.Xml + "lang" || attr.Name == XNamespace.Get("http://www.idpf.org/2007/ops") + "type") continue;
                if (attr.Name.Namespace == XNamespace.None && Regex.IsMatch(attr.Name.LocalName, @"^data-[a-zA-Z0-9_.-]+$", RegexOptions.CultureInvariant)) continue;
                if (element.Name.LocalName == "meta" && attr.Name == "http-equiv"
                    && attr.Value.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) continue;
                if (attr.Name.Namespace != XNamespace.None || !HtmlAttributes.Contains(attr.Name.LocalName))
                    throw Bad("unsafe_content", "Unsupported XHTML attribute.");
                if (attr.Name.LocalName is "href" or "src") Required(entries, Resolve(path, attr.Value));
                if (attr.Name.LocalName == "style") ValidateCss(attr.Value, true);
            }
            if (element.Name.LocalName == "meta" && !IsSafeMeta(element))
                throw Bad("unsafe_content", "Only charset or UTF-8 Content-Type meta is supported.");
            if (element.Name.LocalName == "link" && ((string?)element.Attribute("rel") != "stylesheet"
                || element.Attribute("href") == null)) throw Bad("unsafe_content", "Only local stylesheets are supported.");
            if (element.Name.LocalName == "style") ValidateCss(element.Value, false);
        }
    }

    private static void ValidateNcx(XDocument document, string path, Dictionary<string, ZipArchiveEntry> entries)
    {
        XNamespace ns = "http://www.daisy.org/z3986/2005/ncx/";
        var names = new HashSet<string> { "ncx", "head", "meta", "docTitle", "docAuthor", "text", "navMap", "navPoint", "navLabel", "content", "pageList", "pageTarget", "navList", "navTarget" };
        if (document.Root?.Name != ns + "ncx") throw Bad("invalid_epub", "Invalid NCX.");
        foreach (var element in document.Descendants())
        {
            if (element.Name.Namespace != ns || !names.Contains(element.Name.LocalName)) throw Bad("unsafe_content", "Invalid NCX markup.");
            if (element.Attribute("src") is { } src) Required(entries, Resolve(path, src.Value));
        }
    }

    private static bool IsSafeMeta(XElement meta)
    {
        var charset = (string?)meta.Attribute("charset");
        if (charset != null && !charset.Equals("utf-8", StringComparison.OrdinalIgnoreCase)) return false;
        var httpEquiv = (string?)meta.Attribute("http-equiv");
        if (httpEquiv != null)
            return httpEquiv.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch((string?)meta.Attribute("content") ?? "", @"^(?:text/html|application/xhtml\+xml)\s*;\s*charset\s*=\s*utf-8\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return charset != null || ((string?)meta.Attribute("name") == "calibre:cover"
            && (string?)meta.Attribute("content") is "true" or "false");
    }

    private void ValidateSvgImageWrapper(XElement element, string path, Dictionary<string, ZipArchiveEntry> entries)
    {
        XNamespace svg = "http://www.w3.org/2000/svg";
        XNamespace xlink = "http://www.w3.org/1999/xlink";
        var isRoot = element.Name == svg + "svg" && element.Parent?.Name.Namespace == Html;
        var isImage = element.Name == svg + "image" && element.Parent?.Name == svg + "svg" && !element.HasElements;
        if (!isRoot && !isImage) throw Bad("unsafe_content", "Only an inline SVG raster-image wrapper is supported.");
        if (isRoot && (element.Elements().Count() != 1 || element.Elements().Single().Name != svg + "image"))
            throw Bad("unsafe_content", "SVG must contain a single raster image.");
        foreach (var attr in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
        {
            if (isImage && (attr.Name == "href" || attr.Name == xlink + "href"))
            {
                var target = Required(entries, Resolve(path, attr.Value));
                // The complete archive validation also checks resource media type/signature.
                if (!new[] { ".png", ".jpg", ".jpeg", ".gif" }.Contains(Path.GetExtension(target.FullName).ToLowerInvariant()))
                    throw Bad("unsafe_content", "SVG may reference only a local raster image.");
                ValidateImage(Bytes(target), Path.GetExtension(target.FullName).ToLowerInvariant() switch
                { ".png" => "image/png", ".gif" => "image/gif", _ => "image/jpeg" });
                continue;
            }
            if (attr.Name.Namespace != XNamespace.None || !(isRoot
                ? new[] { "version", "width", "height", "viewBox", "preserveAspectRatio" }
                : new[] { "width", "height", "x", "y" }).Contains(attr.Name.LocalName)
                || !Regex.IsMatch(attr.Value, @"^[a-zA-Z0-9\s.,%+\-]+$", RegexOptions.CultureInvariant))
                throw Bad("unsafe_content", "Unsupported SVG attribute.");
        }
        if (isImage && element.Attribute("href") == null && element.Attribute(xlink + "href") == null)
            throw Bad("unsafe_content", "SVG image must reference a local raster image.");
    }

    private static void ValidateCss(string css, bool declarationsOnly)
    {
        // No network loader is configured. Preserve bytes/CFI; accept only non-fetching CSS.
        // Keep lexical guards: a browser must not reinterpret discarded or escaped tokens as URLs.
        if (css.IndexOfAny(new[] { '\\', '<', '>' }) >= 0 || css.Contains("/*") || css.Contains('!')
            || Regex.Matches(css, @"([a-zA-Z_-][a-zA-Z0-9_-]*)\s*\(").Any(m => !m.Groups[1].Value.Equals("local", StringComparison.OrdinalIgnoreCase)))
            throw Bad("unsupported_css", "CSS escapes, fetching functions and unsupported syntax are not allowed.");
        // This profile is flat. Bound nesting before the general parser handles untrusted input.
        int braces = 0, parentheses = 0;
        foreach (var character in css)
        {
            if (character == '{' && (++braces > 1 || declarationsOnly)
                || character == '}' && --braces < 0
                || character == '(' && ++parentheses > 1
                || character == ')' && --parentheses < 0)
                throw Bad("unsupported_css", "Nested or malformed CSS is not supported.");
        }
        if (braces != 0 || parentheses != 0) throw Bad("unsupported_css", "Unbalanced CSS is not supported.");
        var parser = new CssParser(new CssParserOptions { IsIncludingUnknownDeclarations = true, IsIncludingUnknownRules = true });
        if (declarationsOnly) { Declarations(css); parser.ParseDeclaration(css); return; }
        foreach (var rule in parser.ParseStyleSheet(css).Rules)
        {
            switch (rule)
            {
                case ICssStyleRule style: Declarations(style.Style.CssText, parsed: true); break;
                case ICssPageRule page: Declarations(page.Style.CssText, parsed: true); break;
                case ICssFontFaceRule font:
                    if (string.IsNullOrWhiteSpace(font.Family) || !Regex.IsMatch(font.Source ?? "",
                        "^\\s*local\\(\\s*(?:\"[\\p{L}\\p{N} _.-]+\"|'[\\p{L}\\p{N} _.-]+'|[\\p{L}\\p{N} _.-]+)\\s*\\)(?:\\s*,\\s*local\\(\\s*(?:\"[\\p{L}\\p{N} _.-]+\"|'[\\p{L}\\p{N} _.-]+'|[\\p{L}\\p{N} _.-]+)\\s*\\))*\\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
                        throw Bad("unsupported_css", "Only local system font sources are supported.");
                    break;
                default: throw Bad("unsupported_css", "Unsupported CSS rule.");
            }
        }
    }

    private static void Declarations(string css, bool parsed = false)
    {
        foreach (var declaration in css.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var colon = declaration.IndexOf(':');
            if (colon <= 0 || !CssProperties.Contains(declaration[..colon].Trim())
                || !Regex.IsMatch(declaration[(colon + 1)..], parsed ? "^[\\p{L}\\p{N}\\s#.,%+\\-/\"'()]+$" : "^[\\p{L}\\p{N}\\s#.,%+\\-/\"']+$", RegexOptions.CultureInvariant))
                throw Bad("unsupported_css", "Unsupported CSS declaration.");
        }
    }

    private static void ValidateImage(byte[] data, string type)
    {
        var valid = type switch {
            "image/png" => data.Length >= 24 && data.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            "image/jpeg" => data.Length >= 4 && data[0] == 255 && data[1] == 216 && data[2] == 255,
            "image/gif" => data.Length >= 10 && (Encoding.ASCII.GetString(data, 0, 6) is "GIF87a" or "GIF89a"),
            _ => false
        };
        if (!valid) throw Bad("unsafe_content", "Resource does not match its image media type.");
    }

    private static ReadingLibraryException Bad(string code, string message) => new(422, code, message);
}


