# AuroraScienceHub.Framework.Ocr

Optical Character Recognition (OCR) library based on Tesseract for extracting text from images.

## Overview

Simple interface for performing OCR operations on images using the Tesseract engine. Supports full image text extraction and region-specific text recognition.

## Key Features

- **Text Extraction** - Extract text from images using Tesseract
- **Region-Based OCR** - Read text from specific image regions
- **Multi-Language Support** - Support for multiple languages via tessdata
- **Easy Integration** - Simple dependency injection setup

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Ocr
```

## Prerequisites

### Tesseract Installation

- **Linux**: `sudo apt update && apt-get install libleptonica-dev libtesseract-dev`
- **macOS**: `brew install leptonica tesseract`
- **Windows**: Download from [Tesseract at UB Mannheim](https://github.com/UB-Mannheim/tesseract/wiki) or `choco install tesseract`

### Language Data

Place `.traineddata` files in `./tessdata` folder. Download from [tessdata repository](https://github.com/tesseract-ocr/tessdata).

## Usage

### Basic Setup

```csharp
builder.Services.AddOcrServices();
```

### Extract Text from Image

```csharp
public class DocumentService
{
    private readonly IImageTextReader _textReader;

    public DocumentService(IImageTextReader textReader)
    {
        _textReader = textReader;
    }

    public string ExtractText(byte[] imageBytes)
    {
        return _textReader.ReadText(imageBytes);
    }

    public string ExtractFromRegion(byte[] imageBytes)
    {
        var region = new Rectangle(x: 100, y: 50, width: 200, height: 30);
        return _textReader.ReadText(imageBytes, region);
    }
}
```

## API

```csharp
public interface IImageTextReader
{
    string ReadText(byte[] imageBytes);
    string ReadText(byte[] imageBytes, Rectangle rectangle);
}

public class Rectangle
{
    public Rectangle(int x, int y, int width, int height);
}
```


## Troubleshooting

- **Tesseract Not Found**: Ensure Tesseract is installed and in PATH
- **Poor Recognition**: Improve image quality or use appropriate language data
- **Performance Issues**: Process specific regions instead of full images

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Http` - For processing remote images
- `AuroraScienceHub.Framework.Exceptions` - Exception handling
