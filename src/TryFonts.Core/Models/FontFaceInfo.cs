namespace TryFonts.Core.Models;

/// <summary>Platform-independent metadata for a face in a system font family.</summary>
public sealed record FontFaceInfo(string StyleName, int Weight, int Width, bool IsItalic);
