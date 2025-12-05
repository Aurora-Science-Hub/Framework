namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Common MIME content types for blob storage
/// </summary>
public static class ContentTypes
{
    public static class Application
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string Pdf = "application/pdf";
        public const string Zip = "application/zip";
        public const string OctetStream = "application/octet-stream";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
    }

    public static class Text
    {
        public const string Plain = "text/plain";
        public const string Html = "text/html";
        public const string Css = "text/css";
        public const string Csv = "text/csv";
    }

    public static class Image
    {
        public const string Jpeg = "image/jpeg";
        public const string Png = "image/png";
        public const string Gif = "image/gif";
        public const string Webp = "image/webp";
        public const string Svg = "image/svg+xml";
    }

    public static class Audio
    {
        public const string Mp3 = "audio/mpeg";
        public const string Wav = "audio/wav";
    }

    public static class Video
    {
        public const string Mp4 = "video/mp4";
        public const string Webm = "video/webm";
    }
}

