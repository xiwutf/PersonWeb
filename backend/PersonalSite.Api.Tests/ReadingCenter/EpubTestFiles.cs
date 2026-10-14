using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace PersonalSite.Api.Tests.ReadingCenter;

internal static class EpubTestFiles
{
    public static byte[] Create(string body = "<p id=\"p1\">Hello reader</p>", string? css = null,
        string? extraPath = null, byte[]? extraData = null, string version = "3.0", string? packageOverride = null)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            Write(zip, "mimetype", Encoding.UTF8.GetBytes("application/epub+zip"), CompressionLevel.NoCompression);
            Write(zip, "META-INF/container.xml", Encoding.UTF8.GetBytes("""
                <container xmlns="urn:oasis:names:tc:opendocument:xmlns:container" version="1.0">
                <rootfiles><rootfile full-path="OPS/book.opf" media-type="application/oebps-package+xml"/></rootfiles></container>
                """));
            Write(zip, "OPS/book.opf", Encoding.UTF8.GetBytes(packageOverride ?? $$"""
                <package xmlns="http://www.idpf.org/2007/opf" version="{{version}}" unique-identifier="uid">
                  <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
                    <dc:identifier id="uid">test-book</dc:identifier><dc:title>测试书籍</dc:title>
                    <dc:creator>作者</dc:creator><dc:language>zh</dc:language>
                  </metadata>
                  <manifest><item id="ch1" href="chapter.xhtml" media-type="application/xhtml+xml"/>
                    {{(css == null ? "" : "<item id=\"css\" href=\"style.css\" media-type=\"text/css\"/>")}}
                  </manifest><spine><itemref idref="ch1"/></spine>
                </package>
                """));
            Write(zip, "OPS/chapter.xhtml", Encoding.UTF8.GetBytes($"<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Chapter</title></head><body>{body}</body></html>"));
            if (css != null) Write(zip, "OPS/style.css", Encoding.UTF8.GetBytes(css));
            if (extraPath != null) Write(zip, extraPath, extraData ?? new byte[] { 1, 2, 3 });
        }
        return stream.ToArray();
    }

    public static void Write(ZipArchive zip, string path, byte[] bytes, CompressionLevel compression = CompressionLevel.Optimal)
    {
        using var output = zip.CreateEntry(path, compression).Open();
        output.Write(bytes);
    }

    public static byte[] ChangePackage(Func<string, string> change)
    {
        using var stream = new MemoryStream();
        stream.Write(Create());
        stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var old = zip.GetEntry("OPS/book.opf")!;
            string xml;
            using (var reader = new StreamReader(old.Open())) xml = reader.ReadToEnd();
            old.Delete();
            Write(zip, "OPS/book.opf", Encoding.UTF8.GetBytes(change(xml)));
        }
        return stream.ToArray();
    }

    public static byte[] WithCover(bool legacy = false)
    {
        using var stream = new MemoryStream();
        stream.Write(Create(version: legacy ? "2.0" : "3.0")); stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var old = zip.GetEntry("OPS/book.opf")!;
            string xml; using (var reader = new StreamReader(old.Open())) xml = reader.ReadToEnd();
            old.Delete();
            xml = xml.Replace("</manifest>", "<item id='cover' href='cover.png' media-type='image/png' " + (legacy ? "" : "properties='cover-image'") + "/></manifest>");
            if (legacy) xml = xml.Replace("</metadata>", "<meta name='cover' content='cover'/></metadata>");
            Write(zip, "OPS/book.opf", Encoding.UTF8.GetBytes(xml));
            Write(zip, "OPS/cover.png", Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/l1sAAAAASUVORK5CYII="));
        }
        return stream.ToArray();
    }

    public static byte[] WithNcx(string target = "chapter.xhtml#p1", string attributes = "")
    {
        using var stream = new MemoryStream();
        stream.Write(Create(version: "2.0"));
        stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var old = zip.GetEntry("OPS/book.opf")!;
            string xml;
            using (var reader = new StreamReader(old.Open())) xml = reader.ReadToEnd();
            old.Delete();
            xml = xml.Replace("</manifest>", "<item id='ncx' href='toc.ncx' media-type='application/x-dtbncx+xml'/></manifest>")
                .Replace("<spine>", "<spine toc='ncx'>");
            Write(zip, "OPS/book.opf", Encoding.UTF8.GetBytes(xml));
            Write(zip, "OPS/toc.ncx", Encoding.UTF8.GetBytes($"""
                <ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1" {attributes}>
                  <head/><docTitle><text>Book</text></docTitle><navMap>
                    <navPoint id="chapter1" playOrder="1"><navLabel><text>Chapter</text></navLabel>
                      <content src="{target}"/></navPoint>
                  </navMap>
                </ncx>
                """));
        }
        return stream.ToArray();
    }
}

internal sealed class ReadingTestEnvironment : IWebHostEnvironment, IDisposable
{
    private readonly string _temporaryRoot = Path.Combine(Path.GetTempPath(), "PersonWeb-ReadingTests", Guid.NewGuid().ToString("N"));
    public string ApplicationName { get; set; } = typeof(EpubTestFiles).Assembly.GetName().Name!;
    public string EnvironmentName { get; set; } = "Testing";
    public string WebRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string PrivateRoot => Path.Combine(ContentRootPath, "App_Data", "reading-private");

    public ReadingTestEnvironment()
    {
        ContentRootPath = Path.Combine(_temporaryRoot, "backend", "PersonalSite.Api");
        WebRootPath = Path.Combine(ContentRootPath, "wwwroot");
        Directory.CreateDirectory(WebRootPath);
    }

    public void Dispose()
    {
        var expectedParent = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "PersonWeb-ReadingTests")) + Path.DirectorySeparatorChar;
        if (!Path.GetFullPath(_temporaryRoot).StartsWith(expectedParent, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid test cleanup target.");
        if (Directory.Exists(_temporaryRoot)) Directory.Delete(_temporaryRoot, true);
    }
}
