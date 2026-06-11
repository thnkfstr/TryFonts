namespace TryFonts.Core.Models;

/// <summary>
/// Font face styles that may be available for a given font family.
/// </summary>
[Flags]
public enum FontFaceStyle
{
    None = 0,
    Regular = 1 << 0,
    Bold = 1 << 1,
    Italic = 1 << 2,
    BoldItalic = 1 << 3,
}
