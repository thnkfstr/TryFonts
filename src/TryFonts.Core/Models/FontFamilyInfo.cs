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
    /// <summary>Returns true if the given style is confirmed available for this family.</summary>
    public bool HasStyle(FontFaceStyle style) => AvailableStyles.Contains(style);
}
