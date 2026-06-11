using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>
/// Stateless font family name filtering.
/// All comparisons are case-insensitive. An empty or null search term returns all fonts.
/// </summary>
public static class FontFilter
{
    /// <summary>
    /// Applies <paramref name="searchText"/> to <paramref name="fonts"/> using <paramref name="mode"/>.
    /// </summary>
    public static IEnumerable<FontFamilyInfo> Apply(
        IEnumerable<FontFamilyInfo> fonts,
        string? searchText,
        SearchMode mode)
    {
        if (string.IsNullOrEmpty(searchText))
            return fonts;

        return mode switch
        {
            SearchMode.StartsWith => fonts.Where(f =>
                f.FamilyName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase)),

            SearchMode.Contains or _ => fonts.Where(f =>
                f.FamilyName.Contains(searchText, StringComparison.OrdinalIgnoreCase)),
        };
    }
}
