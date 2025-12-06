using System.Collections.Frozen;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Provides automatic content type (MIME type) resolution based on file extensions.
/// </summary>
/// <remarks>
/// <para>
/// This resolver supports a wide range of common file types including documents, images, audio, video,
/// archives, Microsoft Office documents, and fonts. If the extension is not recognized, it defaults to "application/octet-stream".
/// </para>
/// <para>
/// The resolution is case-insensitive and uses a comprehensive mapping of file extensions to MIME types
/// as defined by IANA and common web standards.
/// </para>
/// <para>
/// For more information about MIME types, see <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types/Common_types"/>.
/// </para>
/// </remarks>
internal static class ContentTypeResolver
{
    private static readonly IReadOnlyDictionary<string, string> s_extensionToContentType =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Documents
            [".pdf"] = ContentTypes.Application.Pdf,
            [".json"] = ContentTypes.Application.Json,
            [".xml"] = ContentTypes.Application.Xml,
            [".txt"] = ContentTypes.Text.Plain,
            [".html"] = ContentTypes.Text.Html,
            [".htm"] = ContentTypes.Text.Html,
            [".css"] = ContentTypes.Text.Css,
            [".csv"] = ContentTypes.Text.Csv,

            // Archives
            [".zip"] = ContentTypes.Application.Zip,
            [".7z"] = ContentTypes.Application.SevenZip,
            [".rar"] = ContentTypes.Application.Rar,
            [".tar"] = ContentTypes.Application.Tar,
            [".gz"] = ContentTypes.Application.Gzip,

            // Images
            [".jpg"] = ContentTypes.Image.Jpeg,
            [".jpeg"] = ContentTypes.Image.Jpeg,
            [".png"] = ContentTypes.Image.Png,
            [".gif"] = ContentTypes.Image.Gif,
            [".webp"] = ContentTypes.Image.Webp,
            [".svg"] = ContentTypes.Image.Svg,
            [".bmp"] = ContentTypes.Image.Bmp,
            [".ico"] = ContentTypes.Image.Icon,
            [".tiff"] = ContentTypes.Image.Tiff,
            [".tif"] = ContentTypes.Image.Tiff,

            // Audio
            [".mp3"] = ContentTypes.Audio.Mp3,
            [".wav"] = ContentTypes.Audio.Wav,
            [".ogg"] = ContentTypes.Audio.Ogg,
            [".m4a"] = ContentTypes.Audio.Mp4,
            [".flac"] = ContentTypes.Audio.Flac,
            [".aac"] = ContentTypes.Audio.Aac,

            // Video
            [".mp4"] = ContentTypes.Video.Mp4,
            [".webm"] = ContentTypes.Video.Webm,
            [".avi"] = ContentTypes.Video.Avi,
            [".mov"] = ContentTypes.Video.QuickTime,
            [".wmv"] = ContentTypes.Video.WindowsMedia,
            [".flv"] = ContentTypes.Video.Flash,
            [".mkv"] = ContentTypes.Video.Matroska,
            [".m4v"] = ContentTypes.Video.M4v,

            // Microsoft Office - Legacy
            [".doc"] = ContentTypes.Application.MsWord,
            [".xls"] = ContentTypes.Application.MsExcel,
            [".ppt"] = ContentTypes.Application.MsPowerPoint,

            // Microsoft Office - Modern (OpenXML)
            [".docx"] = ContentTypes.Application.WordDocument,
            [".xlsx"] = ContentTypes.Application.ExcelSpreadsheet,
            [".pptx"] = ContentTypes.Application.PowerPointPresentation,

            // OpenDocument
            [".odt"] = ContentTypes.Application.OpenDocumentText,
            [".ods"] = ContentTypes.Application.OpenDocumentSpreadsheet,
            [".odp"] = ContentTypes.Application.OpenDocumentPresentation,

            // Fonts
            [".woff"] = ContentTypes.Font.Woff,
            [".woff2"] = ContentTypes.Font.Woff2,
            [".ttf"] = ContentTypes.Font.TrueType,
            [".otf"] = ContentTypes.Font.OpenType,
            [".eot"] = ContentTypes.Application.MsFontObject,
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves the MIME content type from a file name based on its extension.
    /// </summary>
    /// <param name="fileName">The file name with extension (e.g., "document.pdf", "image.jpg").</param>
    /// <returns>
    /// The MIME content type corresponding to the file extension.
    /// Returns "application/octet-stream" if the file name is null, empty, has no extension, or the extension is not recognized.
    /// </returns>
    public static string ResolveFromFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return ContentTypes.Application.OctetStream;
        }

        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
        {
            return ContentTypes.Application.OctetStream;
        }

        return s_extensionToContentType.GetValueOrDefault(extension, ContentTypes.Application.OctetStream);
    }
}
