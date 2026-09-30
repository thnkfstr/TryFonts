using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.Core.Tests;

public sealed class FontVariantBuilderTests
{
    private static readonly string Family = SyntheticFontDataGenerator.Generate(1, ["Arial"])[0].FamilyName;

    [Fact]
    public void Build_NarrowFaces_AppearInSearchWithTheirOwnWidthAndStyles()
    {
        var fonts = FontVariantBuilder.Build(Family,
        [
            new("Regular", 400, 5, false),
            new("Bold", 700, 5, false),
            new("Narrow", 400, 3, false),
            new("Narrow Bold", 700, 3, false),
            new("Narrow Italic", 400, 3, true),
            new("Narrow Bold Italic", 700, 3, true),
        ]);

        Assert.Equal(2, fonts.Count);
        var narrow = Assert.Single(FontFilter.Apply(fonts, "nArRoW", SearchMode.Contains));
        Assert.Equal($"{Family} Narrow", narrow.FamilyName);
        Assert.Equal(Family, narrow.RenderFamilyName);
        Assert.Equal(3, narrow.Width);
        Assert.Equal(400, narrow.PreviewWeight(false));
        Assert.Equal(700, narrow.PreviewWeight(true));
        Assert.False(narrow.PreviewItalic(false));
        Assert.True(narrow.PreviewItalic(true));
        Assert.All(new[] { FontFaceStyle.Regular, FontFaceStyle.Bold, FontFaceStyle.Italic, FontFaceStyle.BoldItalic },
            style => Assert.True(narrow.HasStyle(style)));
        Assert.Equal(5, Assert.Single(fonts, font => font.FamilyName == Family).Width);
        Assert.Empty(FontFilter.Apply(fonts, "Narrow", SearchMode.StartsWith));
        Assert.Single(FontFilter.Apply(fonts, $"{Family} Narrow", SearchMode.StartsWith));
    }

    [Fact]
    public void Build_CondensedAndSemiCondensed_RemainDistinct()
    {
        var fonts = FontVariantBuilder.Build(Family,
        [
            new("Regular", 400, 5, false),
            new("Condensed", 400, 3, false),
            new("Condensed Bold", 700, 3, false),
            new("SemiCondensed", 400, 4, false),
            new("Light Condensed", 300, 3, false),
            new("Cond Medium", 400, 3, false),
            new("Cond Demi", 600, 3, false),
        ]);

        var matches = FontFilter.Apply(fonts, "Cond", SearchMode.Contains).ToList();
        Assert.Equal(5, matches.Count);
        Assert.Equal(3, Assert.Single(matches, f => f.FamilyName == $"{Family} Condensed").Width);
        Assert.Equal(4, Assert.Single(matches, f => f.FamilyName == $"{Family} SemiCondensed").Width);
        Assert.Equal(300, Assert.Single(matches, f => f.FamilyName == $"{Family} Light Condensed").Weight);
        Assert.Equal(600, Assert.Single(matches, f => f.FamilyName == $"{Family} Cond Demi").Weight);
    }

    [Fact]
    public void Build_NamedWeights_PreserveTheirDefaultAppearance()
    {
        var fonts = FontVariantBuilder.Build(Family,
        [
            new("Light", 300, 5, false),
            new("Light Italic", 300, 5, true),
            new("Black", 900, 5, false),
            new("Extra Bold", 800, 5, false),
            new("Extra Bold Oblique", 800, 5, true),
        ]);

        Assert.Equal(3, fonts.Count);
        Assert.Equal(300, Assert.Single(fonts, f => f.FamilyName.EndsWith("Light")).PreviewWeight(false));
        var black = Assert.Single(fonts, f => f.FamilyName.EndsWith("Black"));
        Assert.Equal(900, black.PreviewWeight(false));
        Assert.Equal(900, black.PreviewWeight(true));
        Assert.Equal(800, Assert.Single(fonts, f => f.FamilyName.EndsWith("Extra Bold")).Weight);
    }

    [Fact]
    public void Build_ItalicOnlyVariant_KeepsItsDefaultSlant()
    {
        var font = Assert.Single(FontVariantBuilder.Build(Family, [new("Narrow Italic", 400, 3, true)]));
        Assert.True(font.PreviewItalic(false));
        Assert.True(font.PreviewItalic(true));
        Assert.False(font.HasStyle(FontFaceStyle.Regular));
        Assert.True(font.HasStyle(FontFaceStyle.Italic));
    }

    [Fact]
    public void Build_DuplicateAndObliqueFaces_DoNotDuplicateRows()
    {
        var fonts = FontVariantBuilder.Build(Family,
        [
            new("Narrow", 400, 3, false),
            new("narrow", 400, 3, false),
            new("Narrow Oblique", 400, 3, true),
            new("Narrow Italic", 400, 3, true),
        ]);

        var font = Assert.Single(fonts);
        Assert.Equal(2, font.AvailableStyles.Count);
        Assert.False(font.IsItalic);
    }

    [Fact]
    public void Build_VariantAlreadyInFamilyName_DoesNotRepeatIt()
    {
        var font = Assert.Single(FontVariantBuilder.Build($"{Family} Narrow",
            [new("Narrow Regular", 400, 3, false)]));
        Assert.Equal($"{Family} Narrow", font.FamilyName);
    }
}
