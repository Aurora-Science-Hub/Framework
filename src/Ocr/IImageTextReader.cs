namespace AuroraScienceHub.Framework.Ocr;

/// <summary>
/// Interface for reading text from an image (OCR).
/// </summary>
public interface IImageTextReader
{
    /// <summary>
    /// Reads text from an whole image.
    /// </summary>
    string ReadText(byte[] imageBytes);

    /// <summary>
    /// Reads text from a specific rectangle of an image.
    /// </summary>
    string ReadText(byte[] imageBytes, Rectangle rectangle);
}
