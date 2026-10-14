using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class EpubCompatibilityTests
{
    private static EpubImportValidator Validator() => new(Options.Create(new ReadingLibraryOptions()));

    [Fact]
    public void LocalFontsPageMarginsAndTextJustify_PreserveOriginalBytes()
    {
        var bytes = EpubTestFiles.Create(css: """
            @page { margin-top: 5pt; margin-bottom: 5pt; }
            @font-face { font-family: "heiti"; src: local("黑体"), local("System Local"); }
            p { font-family: heiti; text-justify: inter-ideograph; border-top: 1px solid gray; }
            """);
        using var stream = new MemoryStream(bytes);
        Assert.Equal("测试书籍", Validator().Validate(stream).Title);
        Assert.Equal(bytes, stream.ToArray());
    }

    [Theory]
    [InlineData("@font-face {font-family: test; src: url(https://evil.example/font.woff);}")]
    [InlineData("@font-face {font-family: test; src: local('Safe'), url(font.woff);}")]
    [InlineData("@font-face {font-family: test; src: local('Safe') format('woff');}")]
    [InlineData("@font-face {font-family: test; src: local('Safe; url(evil)');}")]
    [InlineData("@font-face {font-family: test; src: l\\6f cal('Safe');}")]
    [InlineData("@page {behavior: evil;}")]
    [InlineData("p {-moz-binding: url(evil);}")]
    [InlineData("@import 'https://evil.example/styles.css';")]
    [InlineData("@media screen {p {color:red;}}")]
    [InlineData("p {color:red;")]
    [InlineData("@font-face {font-family: test; src: local(local(local('Safe')));}")]
    public void UnsafeOrUnsupportedCss_RemainsRejected(string css)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(css: css));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Theory]
    [InlineData("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>")]
    [InlineData("<meta http-equiv='content-type' content='text/html; charset=utf-8'/>")]
    [InlineData("<meta name='calibre:cover' content='true'/>")]
    [InlineData("<p data-AmznRemoved='true' data-AmznRemoved-M8='1'>Text</p>")]
    public void InertPublicationMetadata_IsAccepted(string body)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(body));
        Validator().Validate(stream);
    }

    [Theory]
    [InlineData("<meta http-equiv='refresh' content='0;url=https://evil.example'/>")]
    [InlineData("<meta http-equiv='Content-Type' content='text/html; charset=utf-7'/>")]
    [InlineData("<meta charset='utf-8' http-equiv='Content-Type' content='text/html; charset=utf-7'/>")]
    [InlineData("<meta charset='utf-7'/>")]
    [InlineData("<meta charset='utf-8' http-equiv='refresh' content='0;url=https://evil.example'/>")]
    [InlineData("<p data-test='1' onclick='alert(1)'>Text</p>")]
    public void ActiveMetadataAndEvents_RemainRejected(string body)
    {
        using var stream = new MemoryStream(EpubTestFiles.Create(body));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    [Fact]
    public void InlineSvgWrappingLocalRaster_IsAccepted()
    {
        using var stream = new MemoryStream(WithSvg("<image width='1' height='1' xlink:href='cover.png'/>"));
        Validator().Validate(stream);
    }

    [Theory]
    [InlineData("<image xlink:href='https://evil.example/pixel.png'/>")]
    [InlineData("<image xlink:href='chapter.xhtml'/>")]
    [InlineData("<image href='cover.png' xlink:href='https://evil.example/pixel.png'/>")]
    [InlineData("<svg xmlns='http://www.w3.org/2000/svg'><image xlink:href='cover.png'/></svg>")]
    [InlineData("<image xlink:href='cover.png' onload='alert(1)'/>")]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<foreignObject><div xmlns='http://www.w3.org/1999/xhtml'>Text</div></foreignObject>")]
    [InlineData("<image xlink:href='cover.png'/><animate attributeName='href' values='evil'/>")]
    public void SvgActiveOrRemoteContent_RemainsRejected(string contents)
    {
        using var stream = new MemoryStream(WithSvg(contents));
        Assert.Throws<ReadingLibraryException>(() => Validator().Validate(stream));
    }

    private static byte[] WithSvg(string contents)
    {
        using var stream = new MemoryStream();
        stream.Write(EpubTestFiles.WithCover()); stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            zip.GetEntry("OPS/chapter.xhtml")!.Delete();
            EpubTestFiles.Write(zip, "OPS/chapter.xhtml", Encoding.UTF8.GetBytes($"""
                <html xmlns="http://www.w3.org/1999/xhtml"><head><title>Cover</title></head><body>
                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"
                version="1.1" width="100%" height="100%" viewBox="0 0 1 1" preserveAspectRatio="none">{contents}</svg>
                </body></html>
                """));
        }
        return stream.ToArray();
    }
}
