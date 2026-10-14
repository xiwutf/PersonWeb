using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class CalibreEpubSanitizerTests
{
    [Fact]
    public void SanitizeCss_StripsFunctionsImportantAndAtRules()
    {
        string css = """
            @font-face { font-family: X; src: url(a.ttf); }
            @media screen { p { color: red; } }
            p {
              color: rgb(1, 2, 3) !important;
              font-size: 1.2em;
              background-image: url(cover.png);
              margin: 0;
            }
            """;
        string cleaned = CalibreEpubSanitizer.SanitizeCss(css);
        Assert.DoesNotContain("rgb", cleaned, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("!", cleaned);
        Assert.DoesNotContain("url", cleaned, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@font-face", cleaned, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("font-size: 1.2em", cleaned);
        Assert.Contains("margin: 0", cleaned);
    }

    [Fact]
    public void SanitizeFile_MakesCalibreLikeEpubPassStrictValidator()
    {
        string path = Path.Combine(Path.GetTempPath(), "calibre-sanitize-" + Guid.NewGuid().ToString("N") + ".epub");
        try
        {
            File.WriteAllBytes(path, CreateCalibreLikeEpub());
            CalibreEpubSanitizer.SanitizeFile(path);
            using FileStream stream = File.OpenRead(path);
            EpubMetadata metadata = new EpubImportValidator(Options.Create(new ReadingLibraryOptions())).Validate(stream);
            Assert.Equal("穷查理宝典", metadata.Title);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            if (File.Exists(path + ".sanitized")) File.Delete(path + ".sanitized");
        }
    }

    private static byte[] CreateCalibreLikeEpub()
    {
        using MemoryStream stream = new();
        using (ZipArchive zip = new(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            Write(zip, "mimetype", Encoding.ASCII.GetBytes("application/epub+zip"), CompressionLevel.NoCompression);
            Write(zip, "META-INF/container.xml", Encoding.UTF8.GetBytes("""
                <?xml version="1.0"?>
                <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                  <rootfiles>
                    <rootfile full-path="content.opf" media-type="application/oebps-package+xml"/>
                  </rootfiles>
                </container>
                """));
            Write(zip, "content.opf", Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="utf-8"?>
                <package xmlns="http://www.idpf.org/2007/opf" unique-identifier="id" version="2.0">
                  <metadata xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:opf="http://www.idpf.org/2007/opf">
                    <dc:title>穷查理宝典</dc:title>
                    <dc:creator>Charlie</dc:creator>
                    <dc:language>zh</dc:language>
                    <meta name="cover" content="cover"/>
                  </metadata>
                  <manifest>
                    <item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml"/>
                    <item id="css" href="stylesheet.css" media-type="text/css"/>
                    <item id="font" href="fonts/Book.ttf" media-type="application/x-font-ttf"/>
                    <item id="cover" href="cover.jpg" media-type="image/jpeg"/>
                    <item id="c1" href="chapter.xhtml" media-type="application/xhtml+xml"/>
                  </manifest>
                  <spine toc="ncx">
                    <itemref idref="c1"/>
                  </spine>
                </package>
                """));
            Write(zip, "toc.ncx", Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="utf-8"?>
                <ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1">
                  <head><meta name="dtb:uid" content="id"/></head>
                  <docTitle><text>穷查理宝典</text></docTitle>
                  <navMap>
                    <navPoint id="n1" playOrder="1">
                      <navLabel><text>Start</text></navLabel>
                      <content src="chapter.xhtml"/>
                    </navPoint>
                  </navMap>
                </ncx>
                """));
            Write(zip, "stylesheet.css", Encoding.UTF8.GetBytes("""
                @font-face { font-family: "Book"; src: url(fonts/Book.ttf); }
                .calibre { font-family: Book; color: rgb(0,0,0) !important; margin: 0; font-size: 1em; }
                """));
            Write(zip, "fonts/Book.ttf", new byte[] { 0, 1, 2, 3, 4 });
            // Minimal JPEG: SOI + APP0-ish + EOI is not enough for length; use a tiny valid JPEG header pattern.
            byte[] jpeg =
            [
                0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01,
                0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xD9
            ];
            Write(zip, "cover.jpg", jpeg);
            Write(zip, "chapter.xhtml", Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="utf-8"?>
                <html xmlns="http://www.w3.org/1999/xhtml">
                  <head>
                    <title>穷查理宝典</title>
                    <meta name="calibre:timestamp" content="2020-01-01"/>
                    <link rel="stylesheet" type="text/css" href="stylesheet.css"/>
                    <style type="text/css">p { color: #333 !important; }</style>
                  </head>
                  <body class="calibre">
                    <p style="color: rgb(1,2,3); margin: 1em;">正文</p>
                    <font size="3">旧标签</font>
                    <img src="fonts/Book.ttf" alt="bad"/>
                    <img src="cover.jpg" alt="cover"/>
                  </body>
                </html>
                """));
        }
        return stream.ToArray();
    }

    private static void Write(ZipArchive zip, string path, byte[] bytes,
        CompressionLevel level = CompressionLevel.Optimal)
    {
        ZipArchiveEntry entry = zip.CreateEntry(path, level);
        using Stream stream = entry.Open();
        stream.Write(bytes, 0, bytes.Length);
    }
}
