using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class EpubImportValidatorTests
{
    private static EpubImportValidator Validator(ReadingLibraryOptions? options = null) => new(Options.Create(options ?? new()));

    [Theory]
    [InlineData("2.0")]
    [InlineData("3.0")]
    public void PlainReflowableEpub_ReadsMetadataWithoutChangingBytes(string version)
    {
        var bytes = EpubTestFiles.Create(css: "p { color: #333; font-size: 1em; margin: 0; }", version: version);
        using var stream = new MemoryStream(bytes);
        var metadata = Validator().Validate(stream);
        Assert.Equal("测试书籍", metadata.Title);
        Assert.Equal("作者", metadata.Author);
        Assert.Equal("zh", metadata.Language);
        Assert.Equal(bytes, stream.ToArray());
    }

    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<p onclick=\"alert(1)\">text</p>")]
    [InlineData("<iframe src=\"chapter.xhtml\"/>")]
    [InlineData("<img src=\"https://tracker.example/pixel.png\"/>")]
    [InlineData("<a href=\"javascript:alert(1)\">link</a>")]
    [InlineData("<a href=\"//tracker.example/\">link</a>")]
    [InlineData("<p xml:base=\"https://tracker.example/\">text</p>")]
    [InlineData("<meta http-equiv=\"refresh\" content=\"0;url=https://evil.example/\"/>")]
    [InlineData("<svg xmlns=\"http://www.w3.org/2000/svg\"><script/></svg>")]
    public void ActiveOrRemoteXhtml_IsRejected(string body)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(body));
        var error = Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
        Assert.Equal(422, error.StatusCode);
    }

    [Theory]
    [InlineData("../escape.txt")]
    [InlineData("/absolute.txt")]
    [InlineData("OPS/../escape.txt")]
    [InlineData("OPS\\escape.txt")]
    [InlineData("C:/escape.txt")]
    [InlineData("OPS/%2e%2e/escape.txt")]
    [InlineData("OPS/chapter.xhtml")]
    public void UnsafeOrDuplicateArchivePaths_AreRejected(string path)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(extraPath: path));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Theory]
    [InlineData("@import 'https://evil.example/style.css';")]
    [InlineData("p { background-image: url(https://evil.example/a); }")]
    [InlineData("p { color: expre\\73sion(alert(1)); }")]
    [InlineData("p { behavior: evil; }")]
    [InlineData("p { color: red; } /* comment */")]
    public void CssOutsideConservativeProfile_IsRejected(string css)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(css: css));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Fact]
    public void DtdAndExternalEntities_AreRejected()
    {
        var xml = "<!DOCTYPE package [<!ENTITY xxe SYSTEM 'file:///etc/passwd'>]><package xmlns='http://www.idpf.org/2007/opf' version='3.0'>&xxe;</package>";
        using var stream = new MemoryStream(EpubTestFiles.Create(packageOverride: xml));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Fact]
    public void XmlStylesheetProcessingInstruction_IsRejected()
    {
        var xml = "<?xml-stylesheet href='https://evil.example/remote.css'?>";
        using var stream = new MemoryStream(EpubTestFiles.Create(packageOverride: xml +
            "<package xmlns='http://www.idpf.org/2007/opf' version='3.0'/>"));
        Assert.Equal("unsafe_content", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("https://evil.example/chapter")]
    [InlineData("//evil.example/chapter")]
    public void ExternalOpfGuide_IsRejected(string href)
    {
        using var stream = new MemoryStream(EpubTestFiles.ChangePackage(xml => xml.Replace("</package>",
            $"<guide><reference type='text' href='{href}'/></guide></package>")));
        Assert.Equal("unsafe_content", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }

    [Fact]
    public void LocalOpfGuide_IsAccepted()
    {
        using var stream = new MemoryStream(EpubTestFiles.ChangePackage(xml => xml.Replace("</package>",
            "<guide><reference type='text' href='chapter.xhtml#p1'/></guide></package>")));
        Assert.Equal("测试书籍", Validator().Validate(stream).Title);
    }

    [Fact]
    public void MissingOpfGuideTarget_IsRejected()
    {
        using var stream = new MemoryStream(EpubTestFiles.ChangePackage(xml => xml.Replace("</package>",
            "<guide><reference type='text' href='missing.xhtml'/></guide></package>")));
        Assert.Equal("invalid_epub", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }

    [Fact]
    public void LocalNcx_IsAccepted()
    {
        using var stream = new MemoryStream(EpubTestFiles.WithNcx());
        Assert.Equal("测试书籍", Validator().Validate(stream).Title);
    }

    [Theory]
    [InlineData("https://evil.example/chapter", "")]
    [InlineData("javascript:alert(1)", "")]
    [InlineData("chapter.xhtml#p1", "xml:base='https://evil.example/'")]
    public void ExternalOrBasedNcx_IsRejected(string target, string attributes)
    {
        using var stream = new MemoryStream(EpubTestFiles.WithNcx(target, attributes));
        Assert.Equal("unsafe_content", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }

    [Theory]
    [InlineData("META-INF/encryption.xml")]
    [InlineData("OPS/hidden.js")]
    public void UnmanifestedOrEncryptedResources_AreRejected(string path)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(extraPath: path));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Fact]
    public void NonZipAndTruncatedArchive_AreRejected()
    {
        using var plain = new MemoryStream(Encoding.UTF8.GetBytes("not epub"));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(plain));
        var bytes = EpubTestFiles.Create();
        using var truncated = new MemoryStream(bytes[..^10]);
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(truncated));
    }

    [Fact]
    public void CompressionRatioLimit_IsEnforced()
    {
        using var stream = new MemoryStream(EpubTestFiles.Create("<p>" + new string('x', 50000) + "</p>"));
        Assert.Equal("archive_limits", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }

    [Theory]
    [InlineData(3, 100000, 100000)]
    [InlineData(100, 20, 100000)]
    [InlineData(100, 100000, 30)]
    public void EntryCountAndExpandedLimits_AreEnforced(int count, long entry, long total)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create());
        var options = new ReadingLibraryOptions { MaxEntries = count, MaxEntryBytes = entry, MaxExpandedBytes = total };
        Assert.Equal("archive_limits", Assert.Throws<ReadingLibraryException>(() => Validator(options).Validate(stream)).Code);
    }

    [Fact]
    public void ArchiveSymlink_IsRejected()
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            var entry = zip.CreateEntry("link");
            entry.ExternalAttributes = unchecked((int)0xA1FF0000);
        }
        stream.Position = 0;
        Assert.Equal("unsafe_archive_path", Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream)).Code);
    }
}
