namespace TryFonts.Core.Models;

/// <summary>
/// Immutable description of a discovered font family and the styles it provides.
/// <para>
/// <see cref="SourcePath"/> is optional metadata; it must not be required for normal
/// operation and must not be shown in the default UI unless a deliberate details affordance
/// is added.
/// </para>
/// </summary>
public sealed record FontFamilyInfo(
    string FamilyName,
    IReadOnlySet<FontFaceStyle> AvailableStyles,
    string? SourcePath = null
)
{
    /// <summary>The system family used to render a named variant.</summary>
    public string RenderFamilyName { get; init; } = FamilyName;

    /// <summary>OpenType width class, from 1 (ultra-condensed) to 9 (ultra-expanded).</summary>
    public int Width { get; init; } = 5;

    /// <summary>The variant's default OpenType weight.</summary>
    public int Weight { get; init; } = 400;

    /// <summary>Whether this variant has only italic or oblique faces.</summary>
    public bool IsItalic { get; init; }

    public int PreviewWeight(bool bold) => bold ? Math.Max(700, Weight) : Weight;

    public bool PreviewItalic(bool italic) => italic || IsItalic;

    /// <summary>Returns true if the given style is confirmed available for this family.</summary>
    public bool HasStyle(FontFaceStyle style) => AvailableStyles.Contains(style);
}
