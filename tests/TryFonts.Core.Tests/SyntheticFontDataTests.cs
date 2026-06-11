using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.Core.Tests;

public sealed class SyntheticFontDataTests
{
    [Fact]
    public void Generate_ProducesExactCount()
    {
        var fonts = SyntheticFontDataGenerator.Generate(100);
        Assert.Equal(100, fonts.Count);
    }

    [Fact]
    public void Generate_LargeCount_ProducesExactCount()
    {
        var fonts = SyntheticFontDataGenerator.Generate(5000);
        Assert.Equal(5000, fonts.Count);
    }

    [Fact]
    public void Generate_Zero_ReturnsEmpty()
    {
        var fonts = SyntheticFontDataGenerator.Generate(0);
        Assert.Empty(fonts);
    }

    [Fact]
    public void Generate_AllHaveNonEmptyName()
    {
        var fonts = SyntheticFontDataGenerator.Generate(50);
        Assert.All(fonts, f => Assert.NotEmpty(f.FamilyName));
    }

    [Fact]
    public void Generate_NamesAreUnique()
    {
        var fonts = SyntheticFontDataGenerator.Generate(200);
        var names = fonts.Select(f => f.FamilyName).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void Generate_IsSortedByName()
    {
        var fonts = SyntheticFontDataGenerator.Generate(100);
        var names = fonts.Select(f => f.FamilyName).ToList();
        var sorted = names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        Assert.Equal(sorted, names);
    }

    [Fact]
    public void Generate_AllHaveAtLeastOneStyle()
    {
        var fonts = SyntheticFontDataGenerator.Generate(50);
        Assert.All(fonts, f => Assert.NotEmpty(f.AvailableStyles));
    }

    [Fact]
    public void Generate_WithRealFamilies_CyclesThroughThem()
    {
        var realFamilies = new List<string> { "Arial", "Georgia" };
        var fonts = SyntheticFontDataGenerator.Generate(10, realFamilies);

        // All names should reference one of the real families somewhere
        Assert.All(fonts, f =>
            Assert.True(
                realFamilies.Any(r => f.FamilyName.Contains(r)),
                $"'{f.FamilyName}' should reference a real family"));
    }

    [Fact]
    public void Generate_WithoutRealFamilies_UsesDefaults()
    {
        // Should not throw when realFamilies is null or empty
        var fonts1 = SyntheticFontDataGenerator.Generate(5, null);
        var fonts2 = SyntheticFontDataGenerator.Generate(5, []);

        Assert.Equal(5, fonts1.Count);
        Assert.Equal(5, fonts2.Count);
    }

    [Fact]
    public void Generate_FontFamilyInfo_HasCorrectRecord()
    {
        var fonts = SyntheticFontDataGenerator.Generate(1);
        var font = fonts[0];

        Assert.IsType<FontFamilyInfo>(font);
        Assert.NotEmpty(font.FamilyName);
        Assert.NotNull(font.AvailableStyles);
    }

    /// <summary>
    /// Performance guard: generating 5,000 synthetic records must complete in under 1 second.
    /// This verifies the generator is suitable for quick test/dev use.
    /// </summary>
    [Fact]
    public void Generate_5000_CompletesUnder1Second()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var fonts = SyntheticFontDataGenerator.Generate(5000);
        sw.Stop();

        Assert.Equal(5000, fonts.Count);
        Assert.True(sw.ElapsedMilliseconds < 1000,
            $"Expected < 1000 ms, got {sw.ElapsedMilliseconds} ms");
    }
}
