using AuroraScienceHub.Framework.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Blobs;

/// <summary>
/// Unit tests for <see cref="ContentTypeResolver"/>
/// </summary>
public class ContentTypeResolverTests
{
    [Theory]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("photo.jpg", "image/jpeg")]
    [InlineData("photo.jpeg", "image/jpeg")]
    [InlineData("image.png", "image/png")]
    [InlineData("image.gif", "image/gif")]
    [InlineData("image.webp", "image/webp")]
    [InlineData("icon.svg", "image/svg+xml")]
    [InlineData("favicon.ico", "image/x-icon")]
    [InlineData("picture.bmp", "image/bmp")]
    public void ResolveFromFileName_ImageAndDocuments_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("video.mp4", "video/mp4")]
    [InlineData("video.webm", "video/webm")]
    [InlineData("video.avi", "video/x-msvideo")]
    [InlineData("video.mov", "video/quicktime")]
    [InlineData("video.mkv", "video/x-matroska")]
    public void ResolveFromFileName_Video_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("song.mp3", "audio/mpeg")]
    [InlineData("sound.wav", "audio/wav")]
    [InlineData("music.ogg", "audio/ogg")]
    [InlineData("track.flac", "audio/flac")]
    [InlineData("audio.m4a", "audio/mp4")]
    public void ResolveFromFileName_Audio_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("archive.zip", "application/zip")]
    [InlineData("package.7z", "application/x-7z-compressed")]
    [InlineData("backup.rar", "application/vnd.rar")]
    [InlineData("files.tar", "application/x-tar")]
    [InlineData("compressed.gz", "application/gzip")]
    public void ResolveFromFileName_Archives_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("data.json", "application/json")]
    [InlineData("config.xml", "application/xml")]
    [InlineData("page.html", "text/html")]
    [InlineData("page.htm", "text/html")]
    [InlineData("style.css", "text/css")]
    [InlineData("data.csv", "text/csv")]
    [InlineData("readme.txt", "text/plain")]
    public void ResolveFromFileName_TextFormats_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("spreadsheet.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("presentation.pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    [InlineData("old-doc.doc", "application/msword")]
    [InlineData("old-sheet.xls", "application/vnd.ms-excel")]
    [InlineData("old-presentation.ppt", "application/vnd.ms-powerpoint")]
    public void ResolveFromFileName_MicrosoftOffice_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("font.woff", "font/woff")]
    [InlineData("font.woff2", "font/woff2")]
    [InlineData("font.ttf", "font/ttf")]
    [InlineData("font.otf", "font/otf")]
    [InlineData("font.eot", "application/vnd.ms-fontobject")]
    public void ResolveFromFileName_Fonts_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("file.xyz")]
    [InlineData("unknown.abc")]
    [InlineData("random.qwerty")]
    [InlineData("noextension")]
    [InlineData("")]
    [InlineData(null)]
    public void ResolveFromFileName_UnknownOrMissingExtension_ReturnsOctetStream(string? fileName)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName ?? string.Empty);

        // Assert
        result.ShouldBe("application/octet-stream");
    }

    [Theory]
    [InlineData("Photo.JPG", "image/jpeg")]
    [InlineData("Document.PDF", "application/pdf")]
    [InlineData("Archive.ZIP", "application/zip")]
    [InlineData("Video.MP4", "video/mp4")]
    [InlineData("SONG.MP3", "audio/mpeg")]
    public void ResolveFromFileName_UppercaseExtensions_IsCaseInsensitive(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Theory]
    [InlineData("Photo.JpG", "image/jpeg")]
    [InlineData("Document.PdF", "application/pdf")]
    [InlineData("Archive.ZiP", "application/zip")]
    public void ResolveFromFileName_MixedCaseExtensions_IsCaseInsensitive(string fileName, string expectedContentType)
    {
        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe(expectedContentType);
    }

    [Fact]
    public void ResolveFromFileName_FileWithMultipleDots_UsesLastExtension()
    {
        // Arrange
        var fileName = "backup.2024-12-05.tar.gz";

        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe("application/gzip");
    }

    [Fact]
    public void ResolveFromFileName_FileWithPath_ExtractsExtensionCorrectly()
    {
        // Arrange
        var fileName = "/path/to/document.pdf";

        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe("application/pdf");
    }

    [Fact]
    public void ResolveFromFileName_WindowsPath_ExtractsExtensionCorrectly()
    {
        // Arrange
        var fileName = @"C:\Users\Documents\photo.jpg";

        // Act
        var result = ContentTypeResolver.ResolveFromFileName(fileName);

        // Assert
        result.ShouldBe("image/jpeg");
    }
}

