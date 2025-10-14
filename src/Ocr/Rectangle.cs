namespace AuroraScienceHub.Framework.Ocr;

/// <summary>
/// Represents a rectangle with a location and size.
/// </summary>
/// <param name="X">X-coordinate of the upper-left corner of the rectangle.</param>
/// <param name="Y">Y-coordinate of the upper-left corner of the rectangle.</param>
/// <param name="Width">Width of the rectangle.</param>
/// <param name="Height">Height of the rectangle.</param>
public sealed record class Rectangle(int X, int Y, int Width, int Height);
