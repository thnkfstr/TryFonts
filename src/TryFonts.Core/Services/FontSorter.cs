using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>Stateless, deterministic font family sorting.</summary>
public static class FontSorter
{
    /// <summary>
    /// Returns <paramref name="fonts"/> in the order specified by <paramref name="mode"/>.
    /// Ordering is stable (equal names preserve original order) and case-insensitive.
    /// </summary>
    public static IEnumerable<FontFamilyInfo> Apply(
        IEnumerable<FontFamilyInfo> fonts,
        SortMode mode)
    {
        return mode switch
        {
            SortMode.NameZA =>
                fonts.OrderByDescending(f => f.FamilyName, StringComparer.OrdinalIgnoreCase),

            SortMode.NameAZ or _ =>
                fonts.OrderBy(f => f.FamilyName, StringComparer.OrdinalIgnoreCase),
        };
    }
}
