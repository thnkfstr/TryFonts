using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>Preserves named font variants while grouping their bold and italic faces.</summary>
public static class FontVariantBuilder
{
    public static IReadOnlyList<FontFamilyInfo> Build(string familyName, IEnumerable<FontFaceInfo> faces)
    {
        var groups = faces.GroupBy(face => VariantName(familyName, face), StringComparer.OrdinalIgnoreCase);
        return groups.Select(group =>
        {
            var preferred = group.OrderBy(face => face.IsItalic)
                .ThenBy(face => Math.Abs(face.Weight - 400)).First();
            var styles = group.Select(face => (face.Weight >= 700, face.IsItalic) switch
            {
                (false, false) => FontFaceStyle.Regular,
                (true, false) => FontFaceStyle.Bold,
                (false, true) => FontFaceStyle.Italic,
                (true, true) => FontFaceStyle.BoldItalic,
            }).ToHashSet();

            return new FontFamilyInfo(group.Key, styles)
            {
                RenderFamilyName = familyName,
                Width = preferred.Width,
                Weight = preferred.Weight,
                IsItalic = preferred.IsItalic,
            };
        }).ToList().AsReadOnly();
    }

    private static string VariantName(string familyName, FontFaceInfo face)
    {
        // Only remove the styles controlled by the toolbar. Keep names such as
        // Narrow, Cond Medium, Light, Black, and Extra Bold searchable.
        var words = face.StyleName.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var variant = string.Join(" ", words.Where(word =>
            !word.Equals("Regular", StringComparison.OrdinalIgnoreCase) &&
            !word.Equals("Normal", StringComparison.OrdinalIgnoreCase) &&
            !word.Equals("Italic", StringComparison.OrdinalIgnoreCase) &&
            !word.Equals("Oblique", StringComparison.OrdinalIgnoreCase) &&
            !(face.Weight == 700 && word.Equals("Bold", StringComparison.OrdinalIgnoreCase))));

        if (variant.Length == 0 || familyName.Equals(variant, StringComparison.OrdinalIgnoreCase) ||
            familyName.EndsWith(" " + variant, StringComparison.OrdinalIgnoreCase))
            return familyName;

        return $"{familyName} {variant}";
    }
}
