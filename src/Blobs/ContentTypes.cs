namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Common MIME content types for blob storage
/// </summary>
public static class ContentTypes
{
    /// <summary>
    /// Application-specific content types
    /// </summary>
    public static class Application
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string Pdf = "application/pdf";
        public const string Zip = "application/zip";
        public const string OctetStream = "application/octet-stream";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string Gzip = "application/gzip";
        public const string SevenZip = "application/x-7z-compressed";
        public const string Rar = "application/vnd.rar";
        public const string Tar = "application/x-tar";

        // Microsoft Office - Legacy formats
        public const string MsWord = "application/msword";
        public const string MsExcel = "application/vnd.ms-excel";
        public const string MsPowerPoint = "application/vnd.ms-powerpoint";
        public const string MsFontObject = "application/vnd.ms-fontobject";

        // Microsoft Office - Modern formats (OpenXML)
        public const string WordDocument = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string ExcelSpreadsheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string PowerPointPresentation = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        // OpenDocument formats
        public const string OpenDocumentText = "application/vnd.oasis.opendocument.text";
        public const string OpenDocumentSpreadsheet = "application/vnd.oasis.opendocument.spreadsheet";
        public const string OpenDocumentPresentation = "application/vnd.oasis.opendocument.presentation";
    }

    /// <summary>
    /// Text content types
    /// </summary>
    public static class Text
    {
        public const string Plain = "text/plain";
        public const string Html = "text/html";
        public const string Css = "text/css";
        public const string Csv = "text/csv";
    }

    /// <summary>
    /// Image content types
    /// </summary>
    public static class Image
    {
        public const string Jpeg = "image/jpeg";
        public const string Png = "image/png";
        public const string Gif = "image/gif";
        public const string Webp = "image/webp";
        public const string Svg = "image/svg+xml";
        public const string Bmp = "image/bmp";
        public const string Icon = "image/x-icon";
        public const string Tiff = "image/tiff";
    }

    /// <summary>
    /// Audio content types
    /// </summary>
    public static class Audio
    {
        public const string Mp3 = "audio/mpeg";
        public const string Wav = "audio/wav";
        public const string Ogg = "audio/ogg";
        public const string Mp4 = "audio/mp4";
        public const string Flac = "audio/flac";
        public const string Aac = "audio/aac";
    }

    /// <summary>
    /// Video content types
    /// </summary>
    public static class Video
    {
        public const string Mp4 = "video/mp4";
        public const string Webm = "video/webm";
        public const string Avi = "video/x-msvideo";
        public const string QuickTime = "video/quicktime";
        public const string WindowsMedia = "video/x-ms-wmv";
        public const string Flash = "video/x-flv";
        public const string Matroska = "video/x-matroska";
        public const string M4v = "video/x-m4v";
    }

    /// <summary>
    /// Font content types
    /// </summary>
    public static class Font
    {
        public const string Woff = "font/woff";
        public const string Woff2 = "font/woff2";
        public const string TrueType = "font/ttf";
        public const string OpenType = "font/otf";
    }
}

