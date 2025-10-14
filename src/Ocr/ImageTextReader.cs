using Microsoft.Extensions.Logging;
using Tesseract;

namespace AuroraScienceHub.Framework.Ocr;

internal sealed class ImageTextReader : IImageTextReader
{
    private const string TesseractDataPath = "tessdata";
    private const string DefaultLanguage = "eng";

    private readonly ILogger<ImageTextReader> _logger;

    public ImageTextReader(ILogger<ImageTextReader> logger)
    {
        _logger = logger;
    }

    public string ReadText(byte[] imageBytes)
    {
        try
        {
            using var engine = new TesseractEngine(TesseractDataPath, DefaultLanguage, EngineMode.Default);
            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img, PageSegMode.SingleBlock);

            var text = page.GetText();

            _logger.LogDebug("Text was successfully read from image. Mean confidence: {MeanConfidence}. Text: {Text}",
                page.GetMeanConfidence(), text);

            return text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading text from image");
            throw;
        }
    }

    public string ReadText(byte[] imageBytes, Rectangle rectangle)
    {
        try
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TesseractDataPath);
            using var engine = new TesseractEngine(dataPath, DefaultLanguage, EngineMode.Default);
            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img, PageSegMode.SingleBlock);

            page.RegionOfInterest = new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            var text = page.GetText();

            _logger.LogDebug("Text was successfully read from image. Mean confidence: {MeanConfidence}. Text: {Text}",
                page.GetMeanConfidence(), text);

            return text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading text from image");
            throw;
        }
    }
}
