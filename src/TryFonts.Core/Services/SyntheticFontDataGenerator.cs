using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>
/// Generates synthetic font records for performance testing.
/// <para>
/// THIS IS A DEVELOPMENT/TEST TOOL ONLY. It must never be exposed as a normal user feature.
/// Activate via the <c>--synthetic-fonts &lt;count&gt;</c> command-line argument.
/// </para>
/// <para>
/// Synthetic entries share real family names for rendering (cycling through
/// <paramref name="realFamilies"/>), but use distinct synthetic family names so the
/// virtual list can be stressed to the target count.
/// </para>
/// </summary>
public static class SyntheticFontDataGenerator
{
    private static readonly IReadOnlySet<FontFaceStyle> AllStyles =
        new HashSet<FontFaceStyle>
        {
            FontFaceStyle.Regular,
            FontFaceStyle.Bold,
            FontFaceStyle.Italic,
            FontFaceStyle.BoldItalic,
        };

    private static readonly IReadOnlySet<FontFaceStyle> RegularOnly =
        new HashSet<FontFaceStyle> { FontFaceStyle.Regular };

    /// <summary>
    /// Produces <paramref name="count"/> synthetic <see cref="FontFamilyInfo"/> records.
    /// The returned list is sorted by <see cref="FontFamilyInfo.FamilyName"/>.
    /// </summary>
    /// <param name="count">Number of synthetic records to produce.</param>
    /// <param name="realFamilies">
    /// Real family names to cycle through for rendering. May be empty; in that case a
    /// fallback name ("Arial" on Windows, "Helvetica" on macOS) is used.
    /// </param>
    public static IReadOnlyList<FontFamilyInfo> Generate(
        int count,
        IReadOnlyList<string>? realFamilies = null)
    {
        var families = realFamilies?.Count > 0
            ? realFamilies
            : (IReadOnlyList<string>)["Arial", "Helvetica", "sans-serif"];

        var result = new List<FontFamilyInfo>(count);
        for (int i = 0; i < count; i++)
        {
            var renderFamily = families[i % families.Count];
            var syntheticName = $"Synthetic Font {i + 1:D5} ({renderFamily})";
            var styles = (i % 3 == 0) ? RegularOnly : AllStyles;
            result.Add(new FontFamilyInfo(syntheticName, styles));
        }

        result.Sort((a, b) =>
            StringComparer.OrdinalIgnoreCase.Compare(a.FamilyName, b.FamilyName));

        return result.AsReadOnly();
    }
}
