using SkiaSharp;
using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.App.Services;

/// <summary>
/// Cross-platform font discovery using SkiaSharp's <see cref="SKFontManager"/>.
/// SkiaSharp is a direct dependency of Avalonia and enumerates system fonts on both
/// Windows (via DirectWrite) and macOS (via CoreText).
/// </summary>
public sealed class SkiaFontDiscoveryService : IFontDiscoveryService
{
    public Task<IReadOnlyList<FontFamilyInfo>> DiscoverAsync(
        CancellationToken cancellationToken = default)
    {
        // Move the work off the UI thread; SkiaSharp font enumeration can be slow
        // when a machine has thousands of fonts.
        return Task.Run<IReadOnlyList<FontFamilyInfo>>(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var manager = SKFontManager.Default;
            var familyNames = manager.GetFontFamilies();

            var result = new List<FontFamilyInfo>(familyNames.Length);

            foreach (var name in familyNames)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var info = BuildFontFamilyInfo(name, manager);
                    if (info is not null)
                        result.Add(info);
                }
                catch
                {
                    // Skip fonts that cannot be inspected; keep going.
                }
            }

            // Deduplicate by name (SkiaSharp may report duplicates on some platforms)
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var deduped = result
                .Where(f => seen.Add(f.FamilyName))
                .OrderBy(f => f.FamilyName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return deduped.AsReadOnly();
        }, cancellationToken);
    }

    private static FontFamilyInfo? BuildFontFamilyInfo(string familyName, SKFontManager manager)
    {
        if (string.IsNullOrWhiteSpace(familyName))
            return null;

        var styles = new HashSet<FontFaceStyle>();

        // Check each style by matching and verifying the resolved family name matches.
        // When SkiaSharp cannot find the requested style it returns a fallback from a
        // different family, so the name check is the reliability gate.

        bool Has(SKFontStyle skStyle)
        {
            using var typeface = manager.MatchFamily(familyName, skStyle);
            return typeface is not null &&
                   string.Equals(typeface.FamilyName, familyName, StringComparison.OrdinalIgnoreCase);
        }

        if (Has(SKFontStyle.Normal))      styles.Add(FontFaceStyle.Regular);
        if (Has(SKFontStyle.Bold))        styles.Add(FontFaceStyle.Bold);
        if (Has(SKFontStyle.Italic))      styles.Add(FontFaceStyle.Italic);
        if (Has(SKFontStyle.BoldItalic))  styles.Add(FontFaceStyle.BoldItalic);

        // If no named styles resolved (can happen for some symbol fonts), treat as Regular.
        if (styles.Count == 0)
            styles.Add(FontFaceStyle.Regular);

        return new FontFamilyInfo(familyName, styles);
    }
}
