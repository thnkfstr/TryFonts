using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.Core.Tests;

public sealed class FontSorterTests
{
    private static FontFamilyInfo Font(string name) =>
        new(name, new HashSet<FontFaceStyle> { FontFaceStyle.Regular });

    private static readonly IReadOnlyList<FontFamilyInfo> Unsorted =
    [
        Font("Zapf Dingbats"),
        Font("Arial"),
        Font("courier new"),   // lowercase to test case-insensitive sort
        Font("Georgia"),
        Font("Helvetica"),
    ];

    [Fact]
    public void NameAZ_SortsAscendingCaseInsensitive()
    {
        var result = FontSorter.Apply(Unsorted, SortMode.NameAZ)
            .Select(f => f.FamilyName)
            .ToList();

        // Expect: Arial, courier new, Georgia, Helvetica, Zapf Dingbats
        Assert.Equal("Arial",          result[0]);
        Assert.Equal("courier new",    result[1]);
        Assert.Equal("Georgia",        result[2]);
        Assert.Equal("Helvetica",      result[3]);
        Assert.Equal("Zapf Dingbats",  result[4]);
    }

    [Fact]
    public void NameZA_SortsDescendingCaseInsensitive()
    {
        var result = FontSorter.Apply(Unsorted, SortMode.NameZA)
            .Select(f => f.FamilyName)
            .ToList();

        Assert.Equal("Zapf Dingbats",  result[0]);
        Assert.Equal("Helvetica",      result[1]);
        Assert.Equal("Georgia",        result[2]);
        Assert.Equal("courier new",    result[3]);
        Assert.Equal("Arial",          result[4]);
    }

    [Fact]
    public void Sort_ProducesAllItems()
    {
        var az = FontSorter.Apply(Unsorted, SortMode.NameAZ).ToList();
        var za = FontSorter.Apply(Unsorted, SortMode.NameZA).ToList();

        Assert.Equal(Unsorted.Count, az.Count);
        Assert.Equal(Unsorted.Count, za.Count);
    }

    [Fact]
    public void Sort_IsDeterministic()
    {
        var first  = FontSorter.Apply(Unsorted, SortMode.NameAZ).Select(f => f.FamilyName).ToList();
        var second = FontSorter.Apply(Unsorted, SortMode.NameAZ).Select(f => f.FamilyName).ToList();
        Assert.Equal(first, second);
    }

    [Fact]
    public void Sort_EmptyList_ReturnsEmpty()
    {
        var result = FontSorter.Apply([], SortMode.NameAZ).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void Sort_SingleItem_ReturnsSingleItem()
    {
        var single = new[] { Font("Helvetica") };
        var result = FontSorter.Apply(single, SortMode.NameZA).ToList();
        Assert.Single(result);
        Assert.Equal("Helvetica", result[0].FamilyName);
    }
}
