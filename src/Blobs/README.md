# AuroraScienceHub.Framework.Blobs

Unified blob storage abstraction with S3-compatible storage support (MinIO, AWS S3).

## Overview

Provides a clean interface for working with binary large objects (BLOBs) using S3-compatible storage systems.

## Key Features

- **S3 Compatible** - Works with MinIO, AWS S3, and other S3-compatible storages
- **Unified Interface** - Single `IBlobClient` abstraction for all operations
- **Streaming Support** - Efficient memory handling for large files
- **Metadata Support** - Store and retrieve custom key-value metadata
- **Content Type Resolution** - Automatic MIME type detection

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Blobs
```

## Usage

### Configuration

```json
{
  "S3": {
    "ServerUrl": "http://localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Bucket": "default-bucket",
    "UseHttps": false
  }
}
```

### Service Registration

```csharp
builder.Services.AddS3Blobs();
```

### Basic Operations

```csharp
public class FileService
{
    private readonly IBlobClient _blobClient;

    // Upload file
    public async Task<BlobId> UploadAsync(Stream stream, string fileName)
    {
        return await _blobClient.AddFileAsync(fileName, stream, "application/pdf");
    }

    // Download file
    public async Task<byte[]> DownloadAsync(BlobId blobId)
    {
        var (metadata, content) = await _blobClient.GetAsync(blobId);
        return content;
    }

    // Stream large files
    public async Task StreamToFileAsync(BlobId blobId, string path)
    {
        await using var content = await _blobClient.GetStreamAsync(blobId);
        await using var file = File.Create(path);
        await content.Stream.CopyToAsync(file);
    }

    // Check existence
    public async Task<bool> ExistsAsync(BlobId blobId)
    {
        return await _blobClient.ExistsAsync(blobId);
    }
}
```

