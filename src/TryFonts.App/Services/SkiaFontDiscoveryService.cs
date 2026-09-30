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
                    result.AddRange(BuildFontFamilyInfos(name, manager, cancellationToken));
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
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

    private static IReadOnlyList<FontFamilyInfo> BuildFontFamilyInfos(
        string familyName, SKFontManager manager, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(familyName))
            return [];

        // A system family can contain named width and weight variants (for example,
        // Arial Narrow is a face of Arial). Matching only four normal-width styles
        // loses those names and renders their previews at the wrong width.
        using var styles = manager.GetFontStyles(familyName);
        var faces = new List<FontFaceInfo>(styles.Count);
        for (int i = 0; i < styles.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var style = styles[i];
            faces.Add(new FontFaceInfo(styles.GetStyleName(i), style.Weight, style.Width,
                style.Slant != SKFontStyleSlant.Upright));
        }

        // Keep families whose style metadata is unavailable (including symbol fonts).
        if (faces.Count == 0)
            return [new FontFamilyInfo(familyName, new HashSet<FontFaceStyle> { FontFaceStyle.Regular })];

        return FontVariantBuilder.Build(familyName, faces);
    }
}
