# Use Magick.NET (MagickImage) for image processing

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr)
* Date: 24.08.2024
* Technical Story: SPACE-114

## Context and Problem Statement

We need to choose a library for image processing in the project.

## Considered Options

* [Magick.NET](https://github.com/dlemstra/Magick.NET)
* [ImageSharp](https://github.com/SixLabors/ImageSharp)
* [SkiaSharp](https://github.com/mono/SkiaSharp)

## Decision Outcome

Chosen option: "Use `Magick.NET (MagickImage)`", because it is a powerful library for image processing with a lot of features and good performance.
It also has great compatibility with different platforms.
Library license (Apache 2.0) is also suitable for the project.
