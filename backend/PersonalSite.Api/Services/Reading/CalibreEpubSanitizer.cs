using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace PersonalSite.Api.Services.Reading;

/// <summary>
/// 将 Calibre 输出收成 PersonWeb strict-v2 可接受的子集（仅用于 MOBI→EPUB 导入路径）。
/// </summary>
public static class CalibreEpubSanitizer
{
    private static readonly XNamespace ContainerNs = "urn:oasis:names:tc:opendocument:xmlns:container";
    private static readonly XNamespace Opf = "http://www.idpf.org/2007/opf";
    private static readonly XNamespace Html = "http://www.w3.org/1999/xhtml";
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly XNamespace Xlink = "http://www.w3.org/1999/xlink";
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/xhtml+xml",
        "application/x-dtbncx+xml",
        "text/css",
        "image/png",
        "image/jpeg",
        "image/gif",
    };
    private static readonly HashSet<string> FontExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".ttf", ".otf", ".woff", ".woff2", ".eot", ".fon", ".svg",
    };
    private static readonly HashSet<string> HtmlElements = new(StringComparer.Ordinal)
    {
        "html", "head", "title", "meta", "link", "style", "body", "div", "span", "p",
        "h1", "h2", "h3", "h4", "h5", "h6", "a", "img", "br", "hr", "em", "strong",
        "b", "i", "u", "s", "small", "sup", "sub", "blockquote", "pre", "code", "ul", "ol",
        "li", "dl", "dt", "dd", "table", "thead", "tbody", "tfoot", "tr", "th", "td", "caption",
        "colgroup", "col", "section", "article", "nav", "aside", "header", "footer", "main",
        "figure", "figcaption", "abbr", "cite", "q", "mark", "ruby", "rt", "rp", "wbr",
    };
    // 主动内容整段删除，勿 unwrap（否则 <script>alert(1)</script> 会留下文本并可能通过校验）。
    private static readonly HashSet<string> RemoveElements = new(StringComparer.Ordinal)
    {
        "script", "iframe", "object", "embed", "form", "input", "button", "textarea", "select",
        "video", "audio", "source", "track", "applet", "base", "noscript",
    };
    private static readonly HashSet<string> HtmlAttributes = new(StringComparer.Ordinal)
    {
        "id", "class", "title", "lang", "dir", "style", "href", "src", "alt", "width", "height",
        "rel", "type", "charset", "name", "content", "colspan", "rowspan", "scope", "start", "value",
    };
    private static readonly HashSet<string> CssProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "color", "background-color", "font-family", "font-size", "font-style", "font-weight",
        "font-variant", "line-height", "text-align", "text-indent", "text-decoration", "text-transform",
        "letter-spacing", "word-spacing", "white-space", "vertical-align", "text-justify", "display", "width", "height",
        "max-width", "max-height", "min-width", "min-height", "margin", "margin-top", "margin-bottom",
        "margin-left", "margin-right", "padding", "padding-top", "padding-bottom", "padding-left",
        "padding-right", "border", "border-top", "border-bottom", "border-left", "border-right",
        "border-width", "border-style", "border-color", "border-collapse", "border-spacing",
        "page-break-before", "page-break-after", "page-break-inside", "break-before", "break-after",
        "break-inside", "orphans", "widows", "list-style-type", "list-style-position",
    };
    private static readonly Regex SafeCssValue = new(
        @"^[\p{L}\p{N}\s#.,%+\-/""']+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex SafeCssValueParsed = new(
        @"^[\p{L}\p{N}\s#.,%+\-/""'()]+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static void SanitizeFile(string epubPath)
    {
        string temp = epubPath + ".sanitized";
        if (File.Exists(temp)) File.Delete(temp);
        using (FileStream input = File.OpenRead(epubPath))
        using (ZipArchive source = new(input, ZipArchiveMode.Read, leaveOpen: false))
        using (FileStream output = File.Create(temp))
        using (ZipArchive destination = new(output, ZipArchiveMode.Create))
        {
            Dictionary<string, ZipArchiveEntry> entries = source.Entries
                .Where(entry => !string.IsNullOrEmpty(entry.FullName) && !entry.FullName.EndsWith('/'))
                .GroupBy(entry => Normalize(entry.FullName), StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

            if (!entries.TryGetValue("mimetype", out _) || !entries.ContainsKey("META-INF/container.xml"))
                throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB is missing required container files.");

            WriteRaw(destination, "mimetype", Encoding.ASCII.GetBytes("application/epub+zip"), CompressionLevel.NoCompression);

            string packagePath = ReadPackagePath(ReadBytes(entries["META-INF/container.xml"]));
            if (!entries.TryGetValue(packagePath, out ZipArchiveEntry? packageEntry))
                throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB package document is missing.");

            XDocument package = LoadXml(ReadBytes(packageEntry));
            StripUnsafeXml(package);
            XElement root = package.Root ?? throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB package is empty.");
            // Drop OPF bindings and unknown OPF children that strict-v2 rejects.
            root.Element(Opf + "bindings")?.Remove();
            XElement manifest = root.Element(Opf + "manifest")
                ?? throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB manifest is missing.");
            XElement? spine = root.Element(Opf + "spine");
            HashSet<string> opfNames = new(StringComparer.Ordinal)
            {
                "package", "metadata", "manifest", "item", "spine", "itemref", "meta", "guide", "reference",
            };
            foreach (XElement element in root.Descendants().ToList())
            {
                if (element.Name.Namespace == XNamespace.Get("http://purl.org/dc/elements/1.1/") && !element.HasElements)
                    continue;
                if (element.Name.Namespace != Opf || !opfNames.Contains(element.Name.LocalName))
                    element.Remove();
            }

            HashSet<string> keep = new(StringComparer.Ordinal) { "mimetype", "META-INF/container.xml", packagePath };
            Dictionary<string, string> pathTypes = new(StringComparer.Ordinal);
            List<XElement> dropItems = new();
            foreach (XElement item in manifest.Elements(Opf + "item").ToList())
            {
                string href = (string?)item.Attribute("href") ?? "";
                string type = (string?)item.Attribute("media-type") ?? "";
                string path = Resolve(packagePath, href);
                string properties = (string?)item.Attribute("properties") ?? "";
                if (properties.Split(' ', StringSplitOptions.RemoveEmptyEntries).Any(p => p is "scripted" or "remote-resources")
                    || !AllowedTypes.Contains(type)
                    || FontExtensions.Contains(Path.GetExtension(path))
                    || !entries.ContainsKey(path)
                    || !pathTypes.TryAdd(path, type))
                {
                    dropItems.Add(item);
                    continue;
                }
                keep.Add(path);
            }
            foreach (XElement item in dropItems) item.Remove();

            // Drop guide refs that point at removed resources.
            XElement? guide = root.Element(Opf + "guide");
            if (guide != null)
            {
                foreach (XElement reference in guide.Elements(Opf + "reference").ToList())
                {
                    string href = (string?)reference.Attribute("href") ?? "";
                    if (!keep.Contains(Resolve(packagePath, href))) reference.Remove();
                }
                if (!guide.Elements().Any()) guide.Remove();
            }

            // Spine must only reference remaining XHTML.
            if (spine != null)
            {
                HashSet<string> ids = manifest.Elements(Opf + "item")
                    .Select(item => (string?)item.Attribute("id") ?? "")
                    .Where(id => id.Length > 0)
                    .ToHashSet(StringComparer.Ordinal);
                string? toc = (string?)spine.Attribute("toc");
                if (toc != null && !ids.Contains(toc)) spine.Attribute("toc")?.Remove();
                foreach (XElement itemref in spine.Elements(Opf + "itemref").ToList())
                {
                    string idref = (string?)itemref.Attribute("idref") ?? "";
                    if (!ids.Contains(idref)) itemref.Remove();
                }
            }

            byte[] packageBytes = Encoding.UTF8.GetBytes(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + package.ToString(SaveOptions.DisableFormatting));
            WriteRaw(destination, "META-INF/container.xml", ReadBytes(entries["META-INF/container.xml"]));
            WriteRaw(destination, packagePath, packageBytes);

            foreach (string path in keep.OrderBy(p => p, StringComparer.Ordinal))
            {
                if (path is "mimetype" or "META-INF/container.xml" || path == packagePath) continue;
                if (!entries.TryGetValue(path, out ZipArchiveEntry? entry)) continue;
                byte[] bytes = ReadBytes(entry);
                string type = pathTypes.TryGetValue(path, out string? mediaType) ? mediaType : "";
                if (type == "text/css")
                    bytes = Encoding.UTF8.GetBytes(SanitizeCss(Encoding.UTF8.GetString(bytes)));
                else if (type == "application/xhtml+xml")
                    bytes = Encoding.UTF8.GetBytes(SanitizeMarkup(Encoding.UTF8.GetString(bytes), path, keep));
                WriteRaw(destination, path, bytes);
            }
        }

        File.Delete(epubPath);
        File.Move(temp, epubPath);
    }

    private static string ReadPackagePath(byte[] containerBytes)
    {
        XDocument document = LoadXml(containerBytes);
        string? href = document.Root?.Element(ContainerNs + "rootfiles")?.Elements(ContainerNs + "rootfile")
            .Select(element => (string?)element.Attribute("full-path"))
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
        if (string.IsNullOrWhiteSpace(href))
            throw new ReadingLibraryException(422, "conversion_failed", "Converted EPUB container has no package path.");
        return Normalize(href);
    }

    public static string SanitizeCss(string css)
    {
        // Rebuild to the validator's flat allowlist: no comments, escapes, !, or functions.
        string text = css ?? "";
        text = Regex.Replace(text, @"/\*.*?\*/", " ", RegexOptions.Singleline);
        List<string> rules = new();
        foreach (Match match in Regex.Matches(text, @"([^{}]+)\{([^{}]*)\}"))
        {
            string selector = match.Groups[1].Value.Trim();
            if (selector.Length == 0 || selector.StartsWith('@')
                || selector.IndexOfAny(new[] { '\\', '<', '>', '!' }) >= 0
                || Regex.IsMatch(selector, @"[a-zA-Z_-][\w-]*\s*\("))
                continue;
            string body = SanitizeDeclarations(match.Groups[2].Value);
            if (body.Length == 0) continue;
            rules.Add(selector + " { " + body + " }");
        }
        return string.Join('\n', rules);
    }

    internal static string SanitizeDeclarations(string declarations)
    {
        List<string> kept = new();
        foreach (string part in (declarations ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            int colon = part.IndexOf(':');
            if (colon <= 0) continue;
            string name = part[..colon].Trim();
            string value = part[(colon + 1)..].Trim();
            value = Regex.Replace(value, @"!\s*important", " ", RegexOptions.IgnoreCase).Trim();
            if (!CssProperties.Contains(name)) continue;
            if (value.IndexOfAny(new[] { '\\', '<', '>', '!' }) >= 0) continue;
            if (Regex.IsMatch(value, @"[a-zA-Z_-][\w-]*\s*\(")) continue; // reject rgb()/url()/calc()
            if (!SafeCssValue.IsMatch(value)) continue;
            kept.Add(name + ": " + value);
        }
        return string.Join("; ", kept);
    }

    internal static string SanitizeMarkup(string html, string path, HashSet<string> keep)
    {
        XDocument document;
        try { document = LoadXml(Encoding.UTF8.GetBytes(html)); }
        catch (XmlException)
        {
            // Last resort: wrap fragments Calibre sometimes emits without namespaces.
            string wrapped = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>x</title></head><body>"
                + html + "</body></html>";
            document = LoadXml(Encoding.UTF8.GetBytes(wrapped));
        }
        StripUnsafeXml(document);

        foreach (XElement element in document.Descendants().ToList())
        {
            if (element.Parent == null) continue;
            if (element.Name.Namespace == Svg)
            {
                if (!IsSafeSvgWrapper(element, path, keep)) element.Remove();
                continue;
            }
            if (element.Name.Namespace != Html || !HtmlElements.Contains(element.Name.LocalName))
            {
                if (RemoveElements.Contains(element.Name.LocalName)) element.Remove();
                else Unwrap(element);
                continue;
            }

            foreach (XAttribute attr in element.Attributes().Where(a => !a.IsNamespaceDeclaration).ToList())
            {
                if (attr.Name == XNamespace.Xml + "lang"
                    || attr.Name == XNamespace.Get("http://www.idpf.org/2007/ops") + "type")
                    continue;
                if (attr.Name.Namespace == XNamespace.None
                    && Regex.IsMatch(attr.Name.LocalName, @"^data-[a-zA-Z0-9_.-]+$", RegexOptions.CultureInvariant))
                    continue;
                if (element.Name.LocalName == "meta" && attr.Name.LocalName == "http-equiv"
                    && attr.Name.Namespace == XNamespace.None) continue;
                if (attr.Name.Namespace != XNamespace.None || !HtmlAttributes.Contains(attr.Name.LocalName))
                {
                    attr.Remove();
                    continue;
                }
                if (attr.Name.LocalName == "style")
                    attr.Value = SanitizeDeclarations(attr.Value);
                if (attr.Name.LocalName is "href" or "src")
                {
                    string? target = TryResolve(path, attr.Value);
                    if (target == null || !keep.Contains(target))
                    {
                        if (element.Name.LocalName is "img" or "link" or "a")
                        {
                            if (element.Name.LocalName == "a") { attr.Remove(); continue; }
                            element.Remove();
                            break;
                        }
                        attr.Remove();
                    }
                }
            }

            if (element.Parent == null) continue;
            if (element.Name.LocalName == "meta" && !IsSafeMeta(element)) { element.Remove(); continue; }
            if (element.Name.LocalName == "link"
                && ((string?)element.Attribute("rel") != "stylesheet" || element.Attribute("href") == null))
            { element.Remove(); continue; }
            if (element.Name.LocalName == "style")
                element.Value = SanitizeCss(element.Value);
        }

        return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + document.ToString(SaveOptions.DisableFormatting);
    }

    private static bool IsSafeSvgWrapper(XElement element, string path, HashSet<string> keep)
    {
        bool isRoot = element.Name == Svg + "svg" && element.Parent?.Name.Namespace == Html;
        bool isImage = element.Name == Svg + "image" && element.Parent?.Name == Svg + "svg" && !element.HasElements;
        if (!isRoot && !isImage) return false;
        if (isRoot && (element.Elements().Count() != 1 || element.Elements().Single().Name != Svg + "image"))
            return false;
        foreach (XAttribute attr in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
        {
            if (isImage && (attr.Name == "href" || attr.Name == Xlink + "href"))
            {
                string? target = TryResolve(path, attr.Value);
                if (target == null || !keep.Contains(target)) return false;
                string ext = Path.GetExtension(target).ToLowerInvariant();
                if (ext is not (".png" or ".jpg" or ".jpeg" or ".gif")) return false;
                continue;
            }
            if (attr.Name.Namespace != XNamespace.None
                || !(isRoot
                    ? new[] { "version", "width", "height", "viewBox", "preserveAspectRatio" }
                    : new[] { "width", "height", "x", "y" }).Contains(attr.Name.LocalName)
                || !Regex.IsMatch(attr.Value, @"^[a-zA-Z0-9\s.,%+\-]+$", RegexOptions.CultureInvariant))
                return false;
        }
        return !isImage || element.Attribute("href") != null || element.Attribute(Xlink + "href") != null;
    }

    private static bool IsSafeMeta(XElement meta)
    {
        string? charset = (string?)meta.Attribute("charset");
        if (charset != null && !charset.Equals("utf-8", StringComparison.OrdinalIgnoreCase)) return false;
        string? httpEquiv = (string?)meta.Attribute("http-equiv");
        if (httpEquiv != null)
            return httpEquiv.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch((string?)meta.Attribute("content") ?? "",
                    @"^(?:text/html|application/xhtml\+xml)\s*;\s*charset\s*=\s*utf-8\s*$",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return charset != null || ((string?)meta.Attribute("name") == "calibre:cover"
            && (string?)meta.Attribute("content") is "true" or "false");
    }

    private static void Unwrap(XElement element)
    {
        foreach (XNode child in element.Nodes().ToList())
            element.AddBeforeSelf(child);
        element.Remove();
    }

    private static void StripUnsafeXml(XDocument document)
    {
        foreach (XProcessingInstruction pi in document.DescendantNodes().OfType<XProcessingInstruction>().ToList())
            pi.Remove();
        foreach (XAttribute attr in document.Descendants()
            .SelectMany(element => element.Attributes(XNamespace.Xml + "base")).ToList())
            attr.Remove();
    }

    private static XDocument LoadXml(byte[] bytes)
    {
        using MemoryStream input = new(bytes);
        using XmlReader reader = XmlReader.Create(input, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = 40 * 1024 * 1024,
            MaxCharactersFromEntities = 0,
        });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private static string? TryResolve(string source, string reference)
    {
        try { return Resolve(source, reference); }
        catch (ReadingLibraryException) { return null; }
    }

    private static string Resolve(string source, string reference)
    {
        if (string.IsNullOrWhiteSpace(reference) || reference.Contains('\\') || reference.Contains(':')
            || reference.StartsWith('/') || reference.Contains('%') || reference.Contains('?')
            || reference.Any(char.IsControl))
            throw new ReadingLibraryException(422, "conversion_failed", "External or ambiguous resource reference.");
        string target = reference.Split('#', 2)[0];
        if (target.Length == 0) return Normalize(source);
        string baseDir = source.Contains('/') ? source[..(source.LastIndexOf('/') + 1)] : "";
        List<string> parts = baseDir.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (string segment in target.Split('/'))
        {
            if (segment == ".") continue;
            if (segment == "..")
            {
                if (parts.Count == 0)
                    throw new ReadingLibraryException(422, "conversion_failed", "Resource escapes publication.");
                parts.RemoveAt(parts.Count - 1);
            }
            else if (segment.Length == 0)
                throw new ReadingLibraryException(422, "conversion_failed", "Ambiguous resource reference.");
            else parts.Add(segment);
        }
        return string.Join('/', parts);
    }

    private static string Normalize(string path)
    {
        string[] parts = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        List<string> stack = new();
        foreach (string part in parts)
        {
            if (part == ".") continue;
            if (part == "..")
            {
                if (stack.Count == 0) return path.Replace('\\', '/').Trim('/');
                stack.RemoveAt(stack.Count - 1);
                continue;
            }
            stack.Add(part);
        }
        return string.Join('/', stack);
    }

    private static byte[] ReadBytes(ZipArchiveEntry entry)
    {
        using Stream stream = entry.Open();
        using MemoryStream memory = new();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static void WriteRaw(ZipArchive archive, string path, byte[] bytes,
        CompressionLevel level = CompressionLevel.Optimal)
    {
        ZipArchiveEntry entry = archive.CreateEntry(path, level);
        using Stream stream = entry.Open();
        stream.Write(bytes, 0, bytes.Length);
    }
}
