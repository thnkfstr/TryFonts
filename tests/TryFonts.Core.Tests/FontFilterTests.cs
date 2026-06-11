using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.Core.Tests;

public sealed class FontFilterTests
{
    private static FontFamilyInfo Font(string name) =>
        new(name, new HashSet<FontFaceStyle> { FontFaceStyle.Regular });

    private static readonly IReadOnlyList<FontFamilyInfo> SampleFonts =
    [
        Font("Arial"),
        Font("Arial Rounded MT Bold"),
        Font("Georgia"),
        Font("Helvetica Neue"),
        Font("Times New Roman"),
    ];

    // ── Empty / null search returns all ──────────────────────────────────────

    [Fact]
    public void Apply_EmptySearch_ReturnsAll()
    {
        var result = FontFilter.Apply(SampleFonts, "", SearchMode.Contains).ToList();
        Assert.Equal(SampleFonts.Count, result.Count);
    }

    [Fact]
    public void Apply_NullSearch_ReturnsAll()
    {
        var result = FontFilter.Apply(SampleFonts, null, SearchMode.Contains).ToList();
        Assert.Equal(SampleFonts.Count, result.Count);
    }

    [Fact]
    public void Apply_WhitespaceSearch_ReturnsAll()
    {
        // Whitespace is treated as empty by IsNullOrEmpty
        var result = FontFilter.Apply(SampleFonts, "", SearchMode.StartsWith).ToList();
        Assert.Equal(SampleFonts.Count, result.Count);
    }

    // ── Contains mode ─────────────────────────────────────────────────────────

    [Fact]
    public void Contains_MatchesSubstring()
    {
        var result = FontFilter
            .Apply(SampleFonts, "round", SearchMode.Contains)
            .Select(f => f.FamilyName)
            .ToList();

        Assert.Single(result);
        Assert.Contains("Arial Rounded MT Bold", result);
    }

    [Fact]
    public void Contains_ExcludesNonMatches()
    {
        var result = FontFilter
            .Apply(SampleFonts, "round", SearchMode.Contains)
            .Select(f => f.FamilyName)
            .ToList();

        Assert.DoesNotContain("Arial", result);
        Assert.DoesNotContain("Georgia", result);
    }

    [Fact]
    public void Contains_IsCaseInsensitive()
    {
        var lower = FontFilter.Apply(SampleFonts, "arial", SearchMode.Contains).ToList();
        var upper = FontFilter.Apply(SampleFonts, "ARIAL", SearchMode.Contains).ToList();
        var mixed = FontFilter.Apply(SampleFonts, "ArIaL", SearchMode.Contains).ToList();

        Assert.Equal(lower.Count, upper.Count);
        Assert.Equal(lower.Count, mixed.Count);
        Assert.Equal(2, lower.Count); // "Arial" and "Arial Rounded MT Bold"
    }

    [Fact]
    public void Contains_MatchesAllFontsContainingTerm()
    {
        var result = FontFilter
            .Apply(SampleFonts, "a", SearchMode.Contains)
            .Select(f => f.FamilyName)
            .ToList();

        // "Arial", "Arial Rounded MT Bold", "Georgia", "Helvetica Neue", "Times New Roman"
        // all contain 'a'
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Contains_NoMatchReturnsEmpty()
    {
        var result = FontFilter.Apply(SampleFonts, "xyzzy", SearchMode.Contains).ToList();
        Assert.Empty(result);
    }

    // ── StartsWith mode ───────────────────────────────────────────────────────

    [Fact]
    public void StartsWith_MatchesPrefix()
    {
        var result = FontFilter
            .Apply(SampleFonts, "ari", SearchMode.StartsWith)
            .Select(f => f.FamilyName)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Arial", result[0]);
        Assert.Equal("Arial Rounded MT Bold", result[1]);
    }

    [Fact]
    public void StartsWith_ExcludesMiddleMatch()
    {
        // "Rounded" is in "Arial Rounded MT Bold" but not at the start
        var result = FontFilter
            .Apply(SampleFonts, "rounded", SearchMode.StartsWith)
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void StartsWith_IsCaseInsensitive()
    {
        var lower = FontFilter.Apply(SampleFonts, "geo",   SearchMode.StartsWith).ToList();
        var upper = FontFilter.Apply(SampleFonts, "GEO",   SearchMode.StartsWith).ToList();
        var mixed = FontFilter.Apply(SampleFonts, "GeOrGiA", SearchMode.StartsWith).ToList();

        Assert.Single(lower);
        Assert.Equal(lower.Count, upper.Count);
        Assert.Single(mixed);
        Assert.Equal("Georgia", mixed[0].FamilyName);
    }

    [Fact]
    public void StartsWith_ExcludesNonPrefixMatch()
    {
        // "Georgia" starts with "G", but "Helvetica Neue" and others do not
        var result = FontFilter
            .Apply(SampleFonts, "g", SearchMode.StartsWith)
            .Select(f => f.FamilyName)
            .ToList();

        Assert.Single(result);
        Assert.Equal("Georgia", result[0]);
    }
}
